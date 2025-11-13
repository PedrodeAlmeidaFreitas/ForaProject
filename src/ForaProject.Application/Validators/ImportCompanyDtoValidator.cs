using FluentValidation;
using ForaProject.Application.DTOs;

namespace ForaProject.Application.Validators;

/// <summary>
/// Validator for ImportCompanyDto.
/// </summary>
public class ImportCompanyDtoValidator : AbstractValidator<ImportCompanyDto>
{
    public ImportCompanyDtoValidator()
    {
        RuleFor(x => x.Cik)
            .GreaterThan(0)
            .WithMessage("CIK must be greater than 0.");
    }
}
