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
            // ViewModel to Entity
            CreateMap<StudentFormViewModel, Student>()
                .ForMember(dest => dest.StudentSubjects, opt => opt.Ignore());

            // Entity to ViewModel
            CreateMap<Student, StudentFormViewModel>()
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());
        }
    }
}
