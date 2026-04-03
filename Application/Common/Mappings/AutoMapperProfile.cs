using AutoMapper;
using StudentMgmt.Application.Commands;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Features.Students;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Common.Mappings;

public class AutoMapperProfile : Profile
{
	public AutoMapperProfile()
	{
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.ClassName : ""));
        CreateMap<CreateStudentCommand, Student>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StudentCode, opt => opt.Ignore())
            .ForMember(dest => dest.Classroom, opt => opt.Ignore())
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.ClassroomId, opt => opt.Condition(src => src.ClassroomId.HasValue));
        CreateMap<UpdateStudentCommand, Student>()

            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StudentCode, opt => opt.Ignore())
            .ForMember(dest => dest.Classroom, opt => opt.Ignore()) 
            .ForMember(dest => dest.BirthDate, opt => {
                opt.Condition(src => src.DateOfBirth.HasValue);
                opt.MapFrom(src => src.DateOfBirth!.Value);
            })
            .ForMember(dest => dest.ClassroomId, opt => opt.Condition(src => src.ClassroomId.HasValue))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}