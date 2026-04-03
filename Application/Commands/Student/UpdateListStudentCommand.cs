using FluentValidation;
using MediatR;
using StudentMgmt.Application.Common.Exceptions;
using StudentMgmt.Application.Interfaces.Repositories;

namespace StudentMgmt.Application.Features.Students;

public record BulkUpdateStudentClassCommand : IRequest<int>
{
    public List<Guid> StudentIds { get; init; } = new();
    public Guid NewClassroomId { get; init; }
}

public class BulkUpdateStudentClassValidator : AbstractValidator<BulkUpdateStudentClassCommand>
{
    public BulkUpdateStudentClassValidator()
    {
        RuleFor(x => x.StudentIds)
            .NotEmpty().WithMessage("Danh sách ID sinh viên không được để trống.")
            .Must(x => x.Count > 0).WithMessage("Phải chọn ít nhất một sinh viên.");

        RuleFor(x => x.NewClassroomId)
            .NotEmpty().WithMessage("ID lớp học mới là bắt buộc.");
    }
}

public class BulkUpdateStudentClassHandler : IRequestHandler<BulkUpdateStudentClassCommand, int>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassroomRepository _classroomRepository;

    public BulkUpdateStudentClassHandler(
        IStudentRepository studentRepository,
        IClassroomRepository classroomRepository)
    {
        _studentRepository = studentRepository;
        _classroomRepository = classroomRepository;
    }

    public async Task<int> Handle(BulkUpdateStudentClassCommand request, CancellationToken cancellationToken)
    {
        var classroom = await _classroomRepository.GetByIdAsync(request.NewClassroomId, cancellationToken)
                        ?? throw new NotFoundException("Classroom", request.NewClassroomId);

        var studentIdStrings = request.StudentIds.Select(id => id.ToString()).ToList();

        int updatedCount = await _studentRepository.BulkUpdateClassroomAsync(
            studentIdStrings,
            request.NewClassroomId.ToString(),
            cancellationToken);

        return updatedCount;
    }
}