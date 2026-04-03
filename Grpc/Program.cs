using Microsoft.AspNetCore.Server.Kestrel.Core;
using StudentMgmt.Application.Common.Mappings;
using StudentMgmt.Application.Interfaces;
using StudentMgmt.Application.Queries;
using StudentMgmt.Grpc.Services;
using StudentMgmt.Infrastructure.Caching;
using StudentMgmt.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5075, o => o.Protocols = HttpProtocols.Http1);

    options.ListenLocalhost(5076, o => o.Protocols = HttpProtocols.Http2);
});

builder.Services.AddMemoryCache();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Chưa cấu hình ConnectionString 'DefaultConnection'!");
builder.Services.AddAutoMapper(
    typeof(AutoMapperProfile).Assembly,
    typeof(GrpcProfile).Assembly
);


builder.Services.AddInfrastructure(connectionString);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(StudentGrpcServiceImplementation).Assembly);
    cfg.RegisterServicesFromAssemblies(typeof(ClassRoomServiceImplementation).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetStudentsQuery).Assembly);
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader()
     .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")));

var app = builder.Build();


app.UseRouting();
app.UseCors("AllowAll");

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.MapGrpcReflectionService();

app.MapGrpcService<StudentGrpcServiceImplementation>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcService<ClassRoomServiceImplementation>().EnableGrpcWeb().RequireCors("AllowAll");

app.MapGet("/", () => "gRPC Server is running with gRPC-Web enabled...");

app.Run();