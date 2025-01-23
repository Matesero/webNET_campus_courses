using courses.Models.DTO;
using FluentValidation;

namespace courses.Validators;

public class EditGroupValidator : AbstractValidator<EditCampusGroupModel>
{
    public EditGroupValidator()
    {
        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(1).WithMessage("Name must be at least 1 characters long");
    }
}