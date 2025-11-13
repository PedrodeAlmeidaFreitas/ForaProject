using FluentValidation;
using ForaProject.Application.DTOs;

namespace ForaProject.Application.Validators;

/// <summary>
/// Validator for BatchImportDto.
/// </summary>
public class BatchImportDtoValidator : AbstractValidator<BatchImportDto>
{
    public BatchImportDtoValidator()
    {
        RuleFor(x => x.Ciks)
            .NotEmpty()
            .WithMessage("At least one CIK is required.");

        RuleForEach(x => x.Ciks)
            .GreaterThan(0)
            .WithMessage("Each CIK must be greater than 0.");
    }
}
