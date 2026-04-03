using StudentMgmt.GrpcServer;
using Google.Protobuf.WellKnownTypes; 
namespace BlazorClient.Services.Student;

public class StudentService : IStudentService
{
    private readonly StudentGrpcService.StudentGrpcServiceClient _client;

    public StudentService(StudentGrpcService.StudentGrpcServiceClient client)
    {
        _client = client;
    }

    public async Task<BulkUpdateResponse> BulkUpdateClassroomAsync(BulkUpdateClassroomRequest request)
    {
        return await _client.BulkUpdateClassroomAsync(request);
    }

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        return await _client.CreateStudentAsync(request);
    }

    public async Task DeleteStudentAsync(DeleteStudentRequest request)
    {
        await _client.DeleteStudentAsync(request);
    }

    public async Task<GetDashboardResponse> GetDashboardAsync()
    {
        try
        {
            return await _client.GetDashboardAsync(new Empty());
        }
        catch (Exception )
        {
            return new GetDashboardResponse
            {
                TotalStudents = 0,
                TotalClasses = 0
            };
        }
    }

    public async Task<PaginatedStudentResponse> GetPagedStudentsAsync(string search, int page, int pageSize)
    {
        var request = new GetStudentsRequest
        {
            SearchCode = search ?? "",
            PageIndex = page,
            PageSize = pageSize
        };
        return await _client.GetPagedStudentsAsync(request);
    }

    public async Task UpdateStudentAsync(UpdateStudentRequest request)
    {
       await _client.UpdateStudentAsync(request);
    }

    async Task<BulkUpdateResponse> IStudentService.BulkUpdateStudentsAsync(BulkUpdateStudentsRequest request)
    {
        return await _client.BulkUpdateStudentsAsync(request);
    }
}