using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using StudentMgmt.Application.Commands;
using StudentMgmt.Application.Commands.Students;
using StudentMgmt.Application.Features.Students;
using StudentMgmt.Application.Queries;
using StudentMgmt.Application.Queries.Students;
using StudentMgmt.GrpcServer;

namespace StudentMgmt.Grpc.Services
{
    public class StudentGrpcServiceImplementation : StudentGrpcService.StudentGrpcServiceBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentGrpcServiceImplementation> _logger;

        public StudentGrpcServiceImplementation(IMediator mediator, ILogger<StudentGrpcServiceImplementation> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<PaginatedStudentResponse> GetPagedStudents(GetStudentsRequest request, ServerCallContext context)
        {
            var result = await _mediator.Send(_mapper.Map<GetStudentsQuery>(request));

            var response = new PaginatedStudentResponse
            {
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                TotalPages = result.TotalPages
            };
            response.Items.AddRange(_mapper.Map<List<StudentResponse>>(result.Items));

            return response;
        }

        public override async Task<StudentResponse> CreateStudent(CreateStudentRequest request, ServerCallContext context)
        {
            var command = _mapper.Map<CreateStudentCommand>(request);
            var result = await _mediator.Send(command);
            return _mapper.Map<StudentResponse>(result);
        }

        public override async Task<StudentResponse> UpdateStudent(UpdateStudentRequest request, ServerCallContext context)
        {
            var command = _mapper.Map<UpdateStudentCommand>(request);
            var result = await _mediator.Send(command);
            return _mapper.Map<StudentResponse>(result);
        }

        public override async Task<DeleteResponse> DeleteStudent(DeleteStudentRequest request, ServerCallContext context)
        {
            var success = await _mediator.Send(new DeleteStudentCommand(Guid.Parse(request.Id)));
            return new DeleteResponse { Success = success };
        }

        public override async Task<BulkUpdateResponse> BulkUpdateClassroom(BulkUpdateClassroomRequest request, ServerCallContext context)
        {
            var command = new BulkUpdateStudentClassCommand
            {
                StudentIds = request.StudentIds.Select(Guid.Parse).ToList(),
                NewClassroomId = Guid.Parse(request.NewClassroomId)
            };

            int updatedCount = await _mediator.Send(command);
            return new BulkUpdateResponse
            {
                Success = updatedCount > 0,
                UpdatedCount = updatedCount,
                Message = updatedCount > 0 ? $"Thành công: Đã chuyển lớp cho {updatedCount} SV." : "Không có SV nào được cập nhật."
            };
        }

        public override async Task<BulkUpdateResponse> BulkUpdateStudents(BulkUpdateStudentsRequest request, ServerCallContext context)
        {
            var command = new BulkUpdateStudentCommand
            {
                StudentIds = request.StudentIds.Select(Guid.Parse).ToList(),
                UpdateMask = request.UpdateMask.ToList(),
                Address = request.Address,
                Status = request.Status,
                ClassroomId = !string.IsNullOrEmpty(request.ClassroomId) ? Guid.Parse(request.ClassroomId) : null
            };

            int updatedCount = await _mediator.Send(command);
            return new BulkUpdateResponse
            {
                Success = true,
                UpdatedCount = updatedCount,
                Message = $"Đã cập nhật thành công {updatedCount} sinh viên."
            };
        }

        public override async Task<GetDashboardResponse> GetDashboard(Empty request, ServerCallContext context)
        {
            var dashboardDto = await _mediator.Send(new GetStudentDashboardQuery());

            return _mapper.Map<GetDashboardResponse>(dashboardDto);
        }
    }
}