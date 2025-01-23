using courses.Models.DTO;
using courses.Models.enums;
using FluentValidation;

namespace courses.Validators;

public class EditCourseValidator : AbstractValidator<EditCampusCourseModel>
{
    public EditCourseValidator()
    {
        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Course name is required")
            .MinimumLength(1).WithMessage("Course name must be at least 1 character");
        
        RuleFor(x => x.startYear)
            .InclusiveBetween(2000, 2029).WithMessage("Start year must be between 2000 and 2029");
        
        RuleFor(x => x.maximumStudentsCount)
            .InclusiveBetween(1, 200).WithMessage("Maximum students count must be between 1 and 200");
        
        RuleFor(x => x.semester)
            .NotEmpty().WithMessage("Semester is required")
            .Must(semester => Enum.IsDefined(typeof(Semesters), semester))
            .WithMessage("Semester is not a valid");

        RuleFor(x => x.requirements)
            .NotEmpty().WithMessage("Requirements are required");

        RuleFor(x => x.annotations)
            .NotEmpty().WithMessage("Annotations are required");
    }
}