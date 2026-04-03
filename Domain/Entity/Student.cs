using StudentMgmt.Domain.Enums;
using System;

namespace StudentMgmt.Domain.Entities
{
    public class Student
    {
        public virtual Guid Id { get; set; }
        public virtual string? StudentCode { get; set; } 
        public virtual string? Name { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string? Address { get; set; }
        public virtual StudentStatus Status { get; set; }
        public virtual Guid? ClassroomId { get; set; }
        public virtual Classroom? Classroom { get; set; }
    }
}