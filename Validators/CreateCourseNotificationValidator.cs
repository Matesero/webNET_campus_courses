using courses.Models.DTO;
using FluentValidation;

namespace courses.Validators;

public class CreateCourseNotificationValidator : AbstractValidator<CampusCourseNotificationModel>
{
    public CreateCourseNotificationValidator()
    {
        RuleFor(x => x.text)
            .NotEmpty().WithMessage("Text is required");
        
        RuleFor(x => x.isImportant)
            .NotEmpty().WithMessage("IsImportant is required");
    }
}