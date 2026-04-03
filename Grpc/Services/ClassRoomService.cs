using Grpc.Core;
using MediatR;
using StudentMgmt.Application.Queries;
using StudentMgmt.GrpcServer;

namespace StudentMgmt.Grpc.Services
{
    public class ClassRoomServiceImplementation : ClassroomGrpcService.ClassroomGrpcServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClassRoomServiceImplementation> _logger;

        public ClassRoomServiceImplementation(IMediator mediator, ILogger<ClassRoomServiceImplementation> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
      
        public override async Task<ClassroomListResponse> GetAllClassrooms(EmptyRequest request, ServerCallContext context)
        {
            var classrooms = await _mediator.Send(new GetClassRoomQuery(), context.CancellationToken);

            var response = new ClassroomListResponse();

            if (classrooms != null && classrooms.Any())
            {
                var grpcItems = classrooms.Select(c => new ClassroomResponse
                {
                    Id = c.Id.ToString(), 
                    ClassName = c.ClassName ?? "N/A"
                });

                response.Items.AddRange(grpcItems);
            }

            return response;
        }
    }
}