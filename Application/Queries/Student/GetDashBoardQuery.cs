using MediatR;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Interfaces;
using StudentMgmt.Application.Interfaces.Repositories;

namespace StudentMgmt.Application.Queries.Students;
public record GetStudentDashboardQuery : IRequest<DashboardDto>;
public class GetStudentDashboardHandler : IRequestHandler<GetStudentDashboardQuery, DashboardDto>
{
    private readonly IStudentRepository _repository;
    private readonly ICacheService _cacheService;

    public GetStudentDashboardHandler(IStudentRepository repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<DashboardDto> Handle(GetStudentDashboardQuery request, CancellationToken ct)
    {
        const string CacheKey = "student_dashboard_stats";

        return (await _cacheService.GetOrCreateAsync(CacheKey, async () =>
        {
            var statsByClass = await _repository.GetCountByClassAsync(ct);
            var statsByStatus = await _repository.GetCountByStatusAsync(ct);

            return new DashboardDto
            {
                StudentsByClass = statsByClass,
                StudentsByStatus = statsByStatus,
                TotalStudents = (int)statsByStatus.Sum(x => x.Value),
                TotalClasses = statsByClass.Count
            };
        }, TimeSpan.FromMinutes(10)))!;
    }
}