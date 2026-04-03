using Microsoft.Extensions.DependencyInjection;
using StudentMgmt.Application.Interfaces;
using StudentMgmt.Application.Interfaces.Common;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Infrastructure.Caching;
using StudentMgmt.Infrastructure.Persistence;
using StudentMgmt.Infrastructure.Repositories;

namespace StudentMgmt.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        var sessionFactory = NHibernateHelper.CreateSessionFactory(connectionString);
        services.AddSingleton(sessionFactory);
        services.AddScoped(factory => sessionFactory.OpenSession());
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IClassroomRepository, ClassRoomRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        return services;
    }
}