using courses.Models.DTO;
using courses.Models.enums;
using FluentValidation;

namespace courses.Validators;

public class EditStudentStatusValidator : AbstractValidator<EditCourseStudentStatusModel>
{
    public EditStudentStatusValidator(){
        RuleFor(x => x.status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => Enum.IsDefined(typeof(StudentStatuses), status))
            .WithMessage("Status is not a valid");
    }
}