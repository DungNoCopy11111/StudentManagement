using AntDesign;
using BlazorClient;
using BlazorClient.Services.Classroom;
using BlazorClient.Services.Student;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StudentMgmt.GrpcServer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAntDesign();
//builder.Services.AddAntDesignCharts();

var serverUrl = "http://localhost:5075";

builder.Services.AddScoped(sp =>
{
    var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
    return GrpcChannel.ForAddress(serverUrl, new GrpcChannelOptions { HttpHandler = handler });
});

builder.Services.AddScoped(sp =>
{
    var channel = sp.GetRequiredService<GrpcChannel>();
    return new StudentGrpcService.StudentGrpcServiceClient(channel);
});

builder.Services.AddScoped(sp =>
{
    var channel = sp.GetRequiredService<GrpcChannel>();
    return new ClassroomGrpcService.ClassroomGrpcServiceClient(channel);
});

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();

await builder.Build().RunAsync();