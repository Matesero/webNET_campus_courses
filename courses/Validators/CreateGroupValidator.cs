using courses.Models.DTO;
using FluentValidation;

namespace courses.Validators;

public class CreateGroupValidator : AbstractValidator<CreateCampusGroupModel>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(1).WithMessage("Name must be at least 1 characters long");
    }
}