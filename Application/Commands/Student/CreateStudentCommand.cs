using AutoMapper;
using FluentValidation;
using MediatR;
using StudentMgmt.Application.Common.Exceptions;
using StudentMgmt.Application.DTOs;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Commands;

// 1. DATA TRANSFER OBJECT (COMMAND)
public class CreateStudentCommand : IRequest<StudentDto>
{
    public string Name { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public string Email { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Address { get; init; } = null!;
    public Guid? ClassroomId { get; init; }
}

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên sinh viên không được để trống.")
            .MaximumLength(100).WithMessage("Tên không được vượt quá 100 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng Email không hợp lệ.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .LessThan(DateTime.Now).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^\d{10,11}$").WithMessage("Số điện thoại phải có 10-11 chữ số.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Địa chỉ không được để trống.");
    }
}

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;

    public CreateStudentCommandHandler(
        IStudentRepository studentRepository,
        IClassroomRepository classroomRepository,
        IMapper mapper)
    {
        _studentRepository = studentRepository;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
    }

    public async Task<StudentDto> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = _mapper.Map<Student>(request);

        if (request.ClassroomId.HasValue)
        {
            var classroom = await _classroomRepository.GetByIdAsync(request.ClassroomId.Value, cancellationToken);

            if (classroom == null)
                throw new NotFoundException("Classroom không tồn tại");

            student.Classroom = classroom;
            student.ClassroomId = classroom.Id;
        }

        int currentCount = await _studentRepository.CountAsync(cancellationToken);
        student.StudentCode = $"SV{DateTime.Now.Year}{(currentCount + 1):D4}";

        await _studentRepository.AddAsync(student, cancellationToken);

        return _mapper.Map<StudentDto>(student);
    }
}