using StudentMgmt.GrpcServer;

namespace BlazorClient.Services.Classroom
{
    public class ClassroomService : IClassroomService
    {
        private readonly ClassroomGrpcService.ClassroomGrpcServiceClient _grpcClient;

        public ClassroomService(ClassroomGrpcService.ClassroomGrpcServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<ClassroomListResponse> GetAllClassroomsAsync(EmptyRequest request)
        {
            return await _grpcClient.GetAllClassroomsAsync(request);
        }
    }
}