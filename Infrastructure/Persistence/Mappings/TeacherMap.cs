using FluentNHibernate.Mapping;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Infrastructure.Persistence.Mappings;

public class TeacherMap : ClassMap<Teacher>
{
    public TeacherMap()
    {
        Table("Teachers");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.TeacherCode).Not.Nullable().Length(50).Unique();
        Map(x => x.Name).Not.Nullable();
        Map(x => x.BirthDate).Nullable();

        HasMany(x => x.Classrooms)
            .Inverse()
            .KeyColumn("TeacherId");
    }
}