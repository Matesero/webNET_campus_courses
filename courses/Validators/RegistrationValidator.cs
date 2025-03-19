using courses.Models.DTO;
using courses.Services;
using FluentValidation;

namespace courses.Validators;

public class RegistrationValidator : AbstractValidator<UserRegisterModel>
{
    private readonly UsersService _usersService;
    
    public RegistrationValidator(UsersService usersService)
    {
        _usersService = usersService;
        
        RuleFor(x => x.fullName)
            .NotEmpty().WithMessage("Fullname is required")
            .MinimumLength(1).WithMessage("Username must be at least 1 characters long");

        RuleFor(x => x.birthDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Birth date cannot be later than today");
        
        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MustAsync(async (email, cancellation) =>
            {
                var user = await _usersService.GetProfileByEmail(email);
                return user is null; 
            })
            .WithMessage("Email is already taken");
        
        
        RuleFor(x => x.password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password length must be at least 6 characters")
            .MaximumLength(32).WithMessage("Password length must be no more than 32 characters")
            .Matches(@"\d").WithMessage("Password requires at least one digit");

        RuleFor(x => x.confirmPassword)
            .Equal(x => x.password).WithMessage("Passwords must be identical")
            .NotEmpty().WithMessage("The ConfirmPassword field is required");
    }
}