using StudentMgmt.Application.DTOs;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<(List<Student> Items, int TotalCount)> GetStudentsAsync(string? searchCode, int pageIndex, int pageSize, CancellationToken ct);
    Task<Student?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Student?> AddAsync(Student student, CancellationToken ct = default);
    Task UpdateAsync(Student student, CancellationToken ct = default);
    Task DeleteAsync(Student student, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> BulkUpdateClassroomAsync(List<string> studentIds, string newClassroomId, CancellationToken ct = default);
    Task<int> ExecuteDynamicBulkUpdateAsync(List<Guid> studentIds, Dictionary<string, object?> updates, CancellationToken ct = default);
    Task<List<StatisticItem>> GetCountByClassAsync(CancellationToken ct);
    Task<List<StatisticItem>> GetCountByStatusAsync(CancellationToken ct);

}