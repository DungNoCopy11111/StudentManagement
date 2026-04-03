using StudentMgmt.GrpcServer;
using Google.Protobuf.WellKnownTypes;

namespace BlazorClient.Services.Student
{
    public interface IStudentService
    {
        Task<PaginatedStudentResponse> GetPagedStudentsAsync(string search, int page, int pageSize);
        Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request);
        Task<BulkUpdateResponse> BulkUpdateClassroomAsync(BulkUpdateClassroomRequest request);
        Task UpdateStudentAsync(UpdateStudentRequest request);
        Task DeleteStudentAsync(DeleteStudentRequest request);
        Task<BulkUpdateResponse> BulkUpdateStudentsAsync(BulkUpdateStudentsRequest request);
        Task<GetDashboardResponse> GetDashboardAsync();
    }
}