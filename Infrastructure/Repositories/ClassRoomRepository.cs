using NHibernate;
using NHibernate.Linq;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMgmt.Infrastructure.Repositories;
public class ClassRoomRepository : IClassroomRepository
{
    private readonly ISession _session;
    public ClassRoomRepository(ISession session)
    {
        _session = session;
    }

    public async Task<List<Classroom>> GetAllAsync(CancellationToken ct = default)
    {
        return await _session.Query<Classroom>().OrderBy(x => x.ClassName).ToListAsync(ct);
    }

    public async Task<Classroom?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _session.Query<Classroom>().FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}

