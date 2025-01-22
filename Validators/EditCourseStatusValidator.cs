using courses.Models.DTO;
using courses.Models.enums;
using FluentValidation;

namespace courses.Validators;

public class EditCourseStatusValidator : AbstractValidator<EditCourseStatusModel>
{
    public EditCourseStatusValidator(){
        RuleFor(x => x.status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => Enum.IsDefined(typeof(CourseStatuses), status))
            .WithMessage("Status is not a valid");
    }
}