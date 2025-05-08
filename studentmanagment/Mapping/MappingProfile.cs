using AutoMapper;
using StudentManagement.Core.Entities;
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
            // Subject mappings
            CreateMap<Subject, SubjectCheckboxItem>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<SubjectCheckboxItem, Subject>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // Student mappings
            CreateMap<Student, StudentFormViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<StudentFormViewModel, Student>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        }
    }
}
