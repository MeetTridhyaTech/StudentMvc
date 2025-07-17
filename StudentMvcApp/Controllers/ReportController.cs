using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using StudentMvcApp.Data;
using System.IO;
using System.Linq;
using System;

using iTextPdf = iText.Kernel.Pdf;
using iTextDoc = iText.Layout.Document;
using iTextParagraph = iText.Layout.Element.Paragraph;
using iTextTable = iText.Layout.Element.Table;
using DocumentFormat.OpenXml;
using StudentMvcApp.Migrations;
using Org.BouncyCastle.Crypto.IO;

public enum ReportFormat
{
    Docx,
    Pdf
}
 
public class ReportController : Controller
{
    private readonly StudentDbContext _context;

    public ReportController(StudentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult DownloadCourseReport(string format)
    {
        if (!Enum.TryParse<ReportFormat>(format, true, out var reportFormat))
        {
            return BadRequest("Invalid format. Use 'pdf' or 'docx'.");
        }

        var courses = _context.Courses
            .Where(c => !c.IsDeleted && c.ParentCourseId == null)
            .Select(c => new
            {
                c.Id,
                c.Name,
                SubCourses = _context.Courses
                    .Where(sc => sc.ParentCourseId == c.Id && !sc.IsDeleted)
                    .Select(sc => new { sc.Id, sc.Name })
                    .ToList()
            }).ToList();

        var now = DateTime.Now;
        var fileName = $"CourseReport_{now:yyyyMMdd_HHmm}";

        switch (reportFormat)
        {
            case ReportFormat.Docx:
                using (MemoryStream docStream = new MemoryStream())
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(docStream, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        body.AppendChild(CreateParagraph("COURSE MANAGEMENT REPORT", true, 22));
                        body.AppendChild(CreateParagraph($"Generated on: {now:yyyy-MM-dd HH:mm}", false, 10));
                        body.AppendChild(new Paragraph(new Run(new Text(""))));

                        foreach (var course in courses)
                        {
                            Table courseTable = new Table();
                            courseTable.AppendChild(new TableProperties(new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )));

                            courseTable.Append(new TableRow(new TableCell(new Paragraph(new Run(new Text("ID")))),
                                                            new TableCell(new Paragraph(new Run(new Text("Course Name"))))));

                            courseTable.Append(new TableRow(
                                CreateTableCell(course.Id.ToString()),
                                CreateTableCell(course.Name)
                            ));

                            body.Append(courseTable);
                            body.Append(CreateParagraph("SubCourses:", true, 14));

                            if (course.SubCourses.Any())
                            {
                                Table subTable = new Table();
                                subTable.AppendChild(new TableProperties(new TableBorders(
                                    new TopBorder { Val = BorderValues.Single, Size = 6 },
                                    new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                    new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                    new RightBorder { Val = BorderValues.Single, Size = 6 },
                                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                                )));

                                subTable.Append(new TableRow(
                                    CreateTableCell("SubCourse ID", true),
                                    CreateTableCell("SubCourse Name", true)
                                ));

                                foreach (var sub in course.SubCourses)
                                {
                                    subTable.Append(new TableRow(
                                        CreateTableCell(sub.Id.ToString()),
                                        CreateTableCell(sub.Name)
                                    ));
                                }

                                body.Append(subTable);
                            }
                            else
                            {
                                body.Append(CreateParagraph("No sub-courses available", false, 12, true));
                            }

                            body.Append(new Paragraph(new Run(new Text(""))));
                        }

                        body.Append(CreateParagraph($"End of Report - Total Courses: {courses.Count}", false, 12, true));
                    }

                    return File(docStream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        $"{fileName}.docx");
                }

            case ReportFormat.Pdf:
                using (var pdfStream = new MemoryStream())
                {
                    var writer = new iTextPdf.PdfWriter(pdfStream);
                    var pdf = new iTextPdf.PdfDocument(writer);
                    var document = new iTextDoc(pdf);

                    document.Add(new iTextParagraph("COURSE MANAGEMENT REPORT").SetBold().SetFontSize(20));
                    document.Add(new iTextParagraph($"Generated on: {now:yyyy-MM-dd HH:mm}").SetFontSize(10));
                    document.Add(new iTextParagraph("\n"));

                    foreach (var course in courses)
                    {
                        document.Add(new iTextParagraph($"Course: {course.Name} (ID: {course.Id})")
                            .SetBold().SetFontSize(14));

                        if (course.SubCourses.Any())
                        {
                            var table = new iTextTable(2, true);
                            table.AddHeaderCell("SubCourse ID");
                            table.AddHeaderCell("SubCourse Name");

                            foreach (var sub in course.SubCourses)
                            {
                                table.AddCell(sub.Id.ToString());
                                table.AddCell(sub.Name);
                            }

                            document.Add(table);
                        }
                        else
                        {
                            document.Add(new iTextParagraph("No sub-courses available")
                                .SetItalic().SetFontSize(12));
                        }

                        document.Add(new iTextParagraph("\n"));
                    }

                    document.Add(new iTextParagraph($"End of Report - Total Courses: {courses.Count}")
                        .SetBold().SetFontSize(12));
                    document.Close();

                    return File(pdfStream.ToArray(),
                        "application/pdf",
                        $"{fileName}.pdf");
                }

            default:
                return BadRequest("Unsupported format.");
        }
    }
    private Paragraph CreateParagraph(string text, bool isBold = false, int fontSize = 12, bool isItalic = false)
    {
        var paragraph = new Paragraph();
        var run = new Run();
        var props = new RunProperties();

        if (isBold) props.Append(new Bold());
        if (isItalic) props.Append(new Italic());
        props.Append(new FontSize() { Val = (fontSize * 2).ToString() });

        run.Append(props);
        run.Append(new Text(text));
        paragraph.Append(run);
        return paragraph;
    }

    private TableCell CreateTableCell(string text, bool isBold = false)
    {
        var runProps = new RunProperties();
        if (isBold) runProps.Append(new Bold());

        var run = new Run(runProps, new Text(text));
        var paragraph = new Paragraph(run);
        return new TableCell(paragraph);
    }
}

