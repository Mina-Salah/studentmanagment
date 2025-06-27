using AutoMapper;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student
            CreateMap<StudentFormViewModel, Student>()
                .ForMember(dest => dest.StudentSubjects, opt => opt.Ignore());

            CreateMap<Student, StudentFormViewModel>()
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());

            // Subject
            CreateMap<Subject, SubjectCheckboxItem>()
                .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => false));
            CreateMap<SubjectCheckboxItem, Subject>();

            // Teacher
            CreateMap<Teacher, TeacherViewModel>().ReverseMap();

            // Category
            CreateMap<Category, CategoryViewModel>().ReverseMap();

            CreateMap<Course, CourseViewModel>()
      .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
      .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<CourseViewModel, Course>();
        }
    }

}
