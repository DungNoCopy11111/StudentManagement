using System.Collections.Generic;

namespace StudentMgmt.Domain.Entities
{
    public class Classroom
    {
        public virtual Guid Id { get; set; }
        public virtual string? ClassCode { get; set; }
        public virtual string? ClassName { get; set; } 
        public virtual string? Subject { get; set; }   
        public virtual Teacher? Teacher { get; set; }
        public virtual IList<Student> Students { get; set; } = new List<Student>();
    }
}