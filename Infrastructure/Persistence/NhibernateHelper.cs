using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using StudentMgmt.Infrastructure.Persistence.Mappings;

namespace StudentMgmt.Infrastructure.Persistence;

public static class NHibernateHelper
{
    public static ISessionFactory CreateSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(connectionString)
                .Driver<NHibernate.Driver.MicrosoftDataSqlClientDriver>())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<StudentMap>())
            .BuildSessionFactory();

    }
}