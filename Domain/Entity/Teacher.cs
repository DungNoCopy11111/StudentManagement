using System;
using System.Collections.Generic;

namespace StudentMgmt.Domain.Entities
{
    public class Teacher
    {
        public virtual Guid Id { get; set; }
        public virtual string? TeacherCode { get; set; }
        public virtual string? Name { get; set; } = string.Empty;
        public virtual DateTime BirthDate { get; set; }
        public virtual IList<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}