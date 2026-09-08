using FluentValidation;
using MiApi.Application.Commands;

namespace MiApi.Application.Validators
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0)
                .WithMessage("CompanyId must be greater than 0");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.PortalId)
                .GreaterThan(0)
                .WithMessage("PortalId must be greater than 0");

            RuleFor(x => x.RoleId)
                .GreaterThan(0)
                .WithMessage("RoleId must be greater than 0");

            RuleFor(x => x.StatusId)
                .GreaterThan(0)
                .WithMessage("StatusId must be greater than 0");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters")
                .MaximumLength(50)
                .WithMessage("Username cannot exceed 50 characters");

            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Name));
        }
    }

    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Employee ID must be greater than 0");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(x => x.RoleId)
                .GreaterThan(0)
                .WithMessage("RoleId must be greater than 0");

            RuleFor(x => x.StatusId)
                .GreaterThan(0)
                .WithMessage("StatusId must be greater than 0");

            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Name));
        }
    }
}
