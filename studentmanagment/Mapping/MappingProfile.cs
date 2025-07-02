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
     .ForMember(dest => dest.TeacherNames, opt => opt.MapFrom(src =>
         string.Join(", ", src.CourseTeachers.Select(ct => ct.Teacher.Name))))
     .ForMember(dest => dest.SelectedTeacherIds, opt => opt.MapFrom(src =>
         src.CourseTeachers.Select(ct => ct.TeacherId)))
     .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<CourseViewModel, Course>()
                .ForMember(dest => dest.CourseTeachers, opt => opt.MapFrom(src =>
                    src.SelectedTeacherIds.Select(tid => new CourseTeacher { TeacherId = tid })))
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // لأنه سيتم ربطه يدويًا غالبًا
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

        }
    }

}
