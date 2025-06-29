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
            CreateMap<Student, StudentFormViewModel>()
        .ForMember(dest => dest.Subjects, opt => opt.Ignore())
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email)); // ✅ هذا السطر مهم

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
      .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.Name))
      .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<CourseViewModel, Course>();
        }
    }

}
