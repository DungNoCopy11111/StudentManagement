using AutoMapper;
using MediatR;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Interfaces.Repositories;

namespace StudentMgmt.Application.Queries;

public record GetStudentsQuery: IRequest<PaginatedList<StudentDto>>
{
    public string? SearchCode { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, PaginatedList<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;
    public GetStudentsQueryHandler(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
    }
    public async Task<PaginatedList<StudentDto>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _studentRepository.GetStudentsAsync(request.SearchCode, request.PageIndex, request.PageSize, cancellationToken);
        var result = _mapper.Map<List<StudentDto>>(items);
        return new PaginatedList<StudentDto>(result, totalCount, request.PageIndex, request.PageSize);
    }
}

