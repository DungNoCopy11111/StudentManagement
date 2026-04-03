using AutoMapper;
using FluentValidation;
using MediatR;
using StudentMgmt.Application.Common.Exceptions;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Features.Students;

public record UpdateStudentCommand : IRequest<StudentDto>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public Guid? ClassroomId { get; init; }
}

public class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID sinh viên là bắt buộc.");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name != null);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => x.Email != null);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(0[3|5|7|8|9])[0-9]{8}$")
            .When(x => x.PhoneNumber != null);

        RuleFor(x => x.Address)
            .MaximumLength(200)
            .When(x => x.Address != null);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .When(x => x.DateOfBirth.HasValue);
    }
}

public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;

    public UpdateStudentHandler(
        IStudentRepository studentRepository,
        IClassroomRepository classroomRepository,
        IMapper mapper)
    {
        _studentRepository = studentRepository;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
    }

    public async Task<StudentDto> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken)
                      ?? throw new NotFoundException("Student", request.Id);


        if (request.Name != null)
            student.Name = request.Name;


        if (request.Address != null)
            student.Address = request.Address;

        if (request.DateOfBirth.HasValue)
            student.BirthDate = request.DateOfBirth.Value;

        if (request.ClassroomId != null)
        {
            if (request.ClassroomId == Guid.Empty)
            {
                student.Classroom = null;
                student.ClassroomId = null;
            }
            else
            {
                var classroom = await _classroomRepository.GetByIdAsync(request.ClassroomId.Value, cancellationToken)
                                ?? throw new NotFoundException("Classroom", request.ClassroomId.Value);

                student.Classroom = classroom;
                student.ClassroomId = classroom.Id;
            }
        }

        await _studentRepository.UpdateAsync(student, cancellationToken);

        return _mapper.Map<StudentDto>(student);
    }
}