using FluentNHibernate.Mapping;
using StudentMgmt.Domain.Entities;
using StudentMgmt.Domain.Enums;

namespace StudentMgmt.Infrastructure.Persistence.Mappings;

public class StudentMap : ClassMap<Student>
{
    public StudentMap()
    {
        Table("Students");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.StudentCode).Not.Nullable().Length(50);
        Map(x => x.Name).Not.Nullable();
        Map(x => x.BirthDate);
        Map(x => x.Address);
        Map(x => x.ClassroomId).Column("ClassroomId").Nullable();
        Map(x => x.Status).CustomType<StudentStatus>().Not.Nullable();
        References(x => x.Classroom)
            .Column("ClassroomId")
            .Not.Insert()
            .Not.Update()
            .Nullable();
    }
}