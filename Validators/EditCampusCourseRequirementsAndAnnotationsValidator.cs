using courses.Models.DTO;
using FluentValidation;

namespace courses.Validators;

public class EditCampusCourseRequirementsAndAnnotationsValidator : AbstractValidator<EditCampusCourseRequirementsAndAnnotationsModel>
{
    public EditCampusCourseRequirementsAndAnnotationsValidator()
    {
        RuleFor(x => x.requirements)
            .NotEmpty().WithMessage("Requirements is required");
        
        RuleFor(x => x.annotations)
            .NotEmpty().WithMessage("Annotations is required");
    }
}