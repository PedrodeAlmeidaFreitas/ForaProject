using FluentValidation;
using ForaProject.Application.DTOs;

namespace ForaProject.Application.Validators;

/// <summary>
/// Validator for CreateCompanyDto.
/// </summary>
public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyDtoValidator()
    {
        RuleFor(x => x.Cik)
            .GreaterThan(0)
            .WithMessage("CIK must be greater than 0.");

        RuleFor(x => x.EntityName)
            .NotEmpty()
            .WithMessage("Entity name is required.")
            .MaximumLength(500)
            .WithMessage("Entity name cannot exceed 500 characters.");
    }
}
