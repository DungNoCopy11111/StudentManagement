using StudentMgmt.GrpcServer;

namespace BlazorClient.Services.Classroom 
{
    public interface IClassroomService
    {
        Task<ClassroomListResponse> GetAllClassroomsAsync(EmptyRequest request);
    }
}