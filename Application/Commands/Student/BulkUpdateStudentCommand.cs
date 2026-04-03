using AutoMapper;
using FluentValidation;
using MediatR;
using StudentMgmt.Application.Common.Exceptions;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Commands.Students;

public record BulkUpdateStudentCommand : IRequest<int>
{
    public List<Guid> StudentIds { get; init; } = new();
    public List<string> UpdateMask { get; init; } = new(); // Chứa: "address", "classroom_id", "status"...

    // Các giá trị tiềm năng
    public string? Address { get; init; }
    public Guid? ClassroomId { get; init; }
    public string? Status { get; init; }
}

public class BulkUpdateStudentHandler : IRequestHandler<BulkUpdateStudentCommand, int>
{
    private readonly IStudentRepository _repository;

    public BulkUpdateStudentHandler(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(BulkUpdateStudentCommand request, CancellationToken ct)
    {
        if (!request.StudentIds.Any() || !request.UpdateMask.Any()) return 0;

        var updates = new Dictionary<string, object?>();

        foreach (var field in request.UpdateMask)
        {
            switch (field.ToLower())
            {
                case "address":
                    updates.Add("s.Address = :address", request.Address);
                    break;
                case "classroom_id":
           
                    if (request.ClassroomId.HasValue)
                    {
                        updates.Add("s.Classroom.Id = :classId", request.ClassroomId.Value);
                    }
                   
                    break;
                case "status":
                    updates.Add("s.Status = :status", request.Status);
                    break;
            }
        }

        // Thực thi Update hàng loạt qua Repository
        return await _repository.ExecuteDynamicBulkUpdateAsync(request.StudentIds, updates, ct);
    }
}