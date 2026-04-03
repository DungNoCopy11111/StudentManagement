using StudentMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMgmt.Application.Interfaces.Repositories
{
    public interface IClassroomRepository
    {
        Task<Classroom?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Classroom>> GetAllAsync(CancellationToken ct = default);
    }
}
