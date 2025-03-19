using courses.Models.DTO;
using FluentValidation;

namespace courses.Validators;

public class LoginValidator : AbstractValidator<UserLoginModel>
{
    
    public LoginValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
        
        RuleFor(x => x.password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password length must be at least 6 characters")
            .MaximumLength(32).WithMessage("Password length must be no more than 32 characters");
    }
}