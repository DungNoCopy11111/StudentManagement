using FluentValidation;
using MediatR;
using StudentMgmt.Application.Common.Exceptions;
using StudentMgmt.Application.Interfaces.Repositories;
using StudentMgmt.Domain.Entities;

namespace StudentMgmt.Application.Features.Students;

public record DeleteStudentCommand(Guid Id) : IRequest<bool>;

public class DeleteStudentValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID sinh viên không được để trống.");
    }
}

public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public DeleteStudentHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (student == null)
        {
            throw new NotFoundException("Student", request.Id);
        }

        await _studentRepository.DeleteAsync(student, cancellationToken);
        return true;
    }
}