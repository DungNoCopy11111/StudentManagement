using MediatR;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Queries;
public record GetClassRoomQuery : IRequest<List<Classroom>>;

public class GetClassRoomQueryHandler : IRequestHandler<GetClassRoomQuery, List<Classroom>>
{
    private readonly IClassroomRepository _classroomRepository;

    public GetClassRoomQueryHandler(IClassroomRepository classroomRepository)
    {
        _classroomRepository = classroomRepository;
    }

    public async Task<List<Classroom>> Handle(GetClassRoomQuery request, CancellationToken cancellationToken)
    {
       
        return await _classroomRepository.GetAllAsync(cancellationToken);
    }
}