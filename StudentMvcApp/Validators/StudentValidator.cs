using FluentValidation;
using StudentMvcApp.Models;

namespace StudentMvcApp.Validators
{
    public class StudentValidator: AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
            RuleFor(s => s.CourseIds)
                .Must(courseIds => courseIds != null && courseIds.Any())
                .WithMessage("At least one course must be selected");
        }
    }
}
