using FluentNHibernate.Mapping;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Infrastructure.Persistence.Mappings;

public class ClassroomMap : ClassMap<Classroom>
{
    public ClassroomMap()
    {
        Table("Classrooms");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.ClassCode).Not.Nullable().Length(20).Unique();
        Map(x => x.ClassName).Not.Nullable().Length(100);
        Map(x => x.Subject);

        // Inverse() báo cho NHibernate rằng phía Student sẽ chịu trách nhiệm lưu khóa ngoại
        // Cascade.All() giúp khi xóa lớp thì có thể xử lý các thực thể con tùy cấu hình
        HasMany(x => x.Students)
            .Inverse() 
            .Cascade.All()
            .KeyColumn("ClassroomId");
    
    }
}