using AutoMapper;
using StudentMgmt.Application.Commands;
using StudentMgmt.Application.Commands.Students;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Features.Students;
using StudentMgmt.Application.Queries;
using StudentMgmt.Application.Queries.Students;
using StudentMgmt.GrpcServer;
using System.Globalization;

public class GrpcProfile : Profile
{
    public GrpcProfile()
    {
        CreateMap<CreateStudentRequest, CreateStudentCommand>()
            .ForMember(d => d.DateOfBirth, o => o.MapFrom(s =>
                DateTime.ParseExact(s.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture)))
            .ForMember(d => d.ClassroomId, o => o.MapFrom(s =>
                string.IsNullOrEmpty(s.ClassroomId) ? (Guid?)null : Guid.Parse(s.ClassroomId)));

        CreateMap<UpdateStudentRequest, UpdateStudentCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                (src.HasName && !string.IsNullOrWhiteSpace(src.Name)) ? src.Name : null))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                (src.HasAddress && !string.IsNullOrWhiteSpace(src.Address)) ? src.Address : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                (src.HasEmail && !string.IsNullOrWhiteSpace(src.Email)) ? src.Email : null))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src =>
                (src.HasPhoneNumber && !string.IsNullOrWhiteSpace(src.PhoneNumber)) ? src.PhoneNumber : null))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src =>
                (src.HasDateOfBirth && !string.IsNullOrWhiteSpace(src.DateOfBirth))
                ? DateTime.ParseExact(src.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : (DateTime?)null))
            .ForMember(dest => dest.ClassroomId, opt => opt.MapFrom(src =>
                (src.HasClassroomId && !string.IsNullOrWhiteSpace(src.ClassroomId))
                ? Guid.Parse(src.ClassroomId)
                : (Guid?)null));

        CreateMap<GetStudentsRequest, GetStudentsQuery>();
        CreateMap<StudentDto, StudentResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.ClassroomId, opt => opt.MapFrom(src => src.ClassroomId.ToString()));


        CreateMap<StudentMgmt.Application.DTOs.StatisticItem, StudentMgmt.GrpcServer.StatisticItem>();

        CreateMap<DashboardDto, GetDashboardResponse>()
            .AfterMap((src, dest) =>
            {
                if (src.StudentsByClass != null)
                {
                    dest.ByClass.AddRange(src.StudentsByClass.Select(x => new StudentMgmt.GrpcServer.StatisticItem
                    {
                        Label = x.Label,
                        Value = x.Value
                    }));
                }

                if (src.StudentsByStatus != null)
                {
                    dest.ByStatus.AddRange(src.StudentsByStatus.Select(x => new StudentMgmt.GrpcServer.StatisticItem
                    {
                        Label = x.Label,
                        Value = x.Value
                    }));
                }
            });

    }
}