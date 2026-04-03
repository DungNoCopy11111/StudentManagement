using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;
using StudentMgmt.Domain.Enums;

namespace StudentMgmt.Infrastructure.Repositories;
public class StudentRepository : IStudentRepository
{
    private readonly ISession _session;
    public StudentRepository(ISession session)
    {
        _session = session;
    }
    public async Task<(List<Student> Items, int TotalCount)> GetStudentsAsync(string? searchCode, int pageIndex, int pageSize, CancellationToken ct)
    {
        var query = _session.Query<Student>();
        if (!string.IsNullOrEmpty(searchCode))
        {
            query = query.Where(s => s.StudentCode != null && s.StudentCode.Contains(searchCode!));
        }
        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(s => s.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, totalCount);
    }

    public async Task DeleteAsync(Student student, CancellationToken ct = default)
    {
        using var transaction = _session.BeginTransaction();
        try
        {
            await _session.DeleteAsync(student, ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Student?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _session.Query<Student>()
            .FirstOrDefaultAsync(s => s.StudentCode == code, ct);
    }

    public async Task UpdateAsync(Student student, CancellationToken ct = default)
    {
        using var transaction = _session.BeginTransaction();
        try
        {
            await _session.UpdateAsync(student, ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Student?> AddAsync(Student student, CancellationToken ct)
    {
        using var transaction = _session.BeginTransaction();
        try
        {
            await _session.SaveAsync(student, ct);
            await transaction.CommitAsync(ct);
            return student;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _session.Query<Student>().CountAsync(ct);
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _session.Query<Student>().FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<int> BulkUpdateClassroomAsync(List<string> studentIds, string newClassroomId, CancellationToken ct)
    {
        if (studentIds == null || !studentIds.Any()) return 0;

        using (var transaction = _session.BeginTransaction())
        {
            try
            {
                var guidIds = studentIds.Select(id => Guid.Parse(id)).ToList();
                var targetClassGuid = Guid.Parse(newClassroomId);
                string hql = "update Student s set s.Classroom.Id = :newClassId where s.Id in (:ids)";

                var query = _session.CreateQuery(hql)
                    .SetParameter("newClassId", targetClassGuid)
                    .SetParameterList("ids", guidIds);


                int updatedCount = await query.ExecuteUpdateAsync(ct);

                await transaction.CommitAsync(ct);
                return updatedCount;
            }
            catch (Exception)
            {
                if (transaction.IsActive)
                {
                    await transaction.RollbackAsync(ct);
                }
                throw; 
            }
        }
    }

    public async Task<int> ExecuteDynamicBulkUpdateAsync(List<Guid> ids, Dictionary<string, object?> updates,CancellationToken ct)
    {
        if (ids == null || !ids.Any() || updates == null || !updates.Any()) return 0;

        using var transaction = _session.BeginTransaction();
        try
        {
            var setClause = string.Join(", ", updates.Keys);

            string hql = $"update Student s set {setClause} where s.Id in (:ids)";

            var query = _session.CreateQuery(hql);

            query.SetParameterList("ids", ids);

            foreach (var update in updates)
            {
                
                var paramName = update.Key.Split(':').Last();
                query.SetParameter(paramName, update.Value);
            }

            int result = await query.ExecuteUpdateAsync(ct);

            await transaction.CommitAsync(ct);
            return result;
        }
        catch (Exception)
        {
            if (transaction.IsActive) await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<List<StatisticItem>> GetCountByClassAsync(CancellationToken ct)
    {
        return (await _session.CreateQuery(@"
            select s.Classroom.ClassName as Label, 
                   count(s.Id) as Value 
            from Student s 
            group by s.Classroom.ClassName")
            .SetResultTransformer(Transformers.AliasToBean<StatisticItem>())
            .ListAsync<StatisticItem>(ct))
            .ToList();
    }

    public async Task<List<StatisticItem>> GetCountByStatusAsync(CancellationToken ct)
    {
        var results = await _session.CreateQuery(@"
        select s.Status, count(s.Id) 
        from Student s 
        group by s.Status")
            .ListAsync<object[]>(ct);

        return results.Select(r => {
            int statusValue = r[0] != null ? Convert.ToInt32(r[0]) : -1;

            return new StatisticItem
            {
               
                Label = statusValue switch
                {
                    0 => "Đang học",    
                    1 => "Đã nghỉ",     
                    2 => "Tốt nghiệp", 
                    _ => "Chưa cập nhật"
                },
                Value = Convert.ToDouble(r[1] ?? 0)
            };
        }).ToList();
    }
}