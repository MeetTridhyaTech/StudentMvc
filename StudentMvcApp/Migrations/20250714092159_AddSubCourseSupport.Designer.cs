﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudentMvcApp.Data;

#nullable disable

namespace StudentMvcApp.Migrations
{
    [DbContext(typeof(StudentDbContext))]
    [Migration("20250714092159_AddSubCourseSupport")]
    partial class AddSubCourseSupport
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("StudentMvcApp.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentCourseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentCourseId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("StudentMvcApp.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.PrimitiveCollection<string>("CourseIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseNames")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("StudentMvcApp.Models.Course", b =>
                {
                    b.HasOne("StudentMvcApp.Models.Course", "ParentCourse")
                        .WithMany("SubCourses")
                        .HasForeignKey("ParentCourseId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentCourse");
                });

            modelBuilder.Entity("StudentMvcApp.Models.Course", b =>
                {
                    b.Navigation("SubCourses");
                });
#pragma warning restore 612, 618
        }
    }
}
