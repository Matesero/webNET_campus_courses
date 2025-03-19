using courses.Models.DTO;
using FluentValidation;

namespace courses.Validators;

public class EditProfileValidator : AbstractValidator<EditUserProfileModel>
{
    public EditProfileValidator()
    {
        RuleFor(x => x.fullName)
            .NotEmpty().WithMessage("Fullname is required")
            .MinimumLength(1).WithMessage("Username must be at least 1 characters long");

        RuleFor(x => x.birthDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Birth date cannot be later than today");
    }
}