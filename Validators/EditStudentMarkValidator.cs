using courses.Models.DTO;
using courses.Models.enums;
using FluentValidation;

namespace courses.Validators;

public class EditStudentMarkValidator : AbstractValidator<EditCourseStudentMarkModel>
{
    public EditStudentMarkValidator(){
        RuleFor(x => x.markType)
            .NotEmpty().WithMessage("Mark type is required")
            .Must(markType => Enum.IsDefined(typeof(MarkType), markType))
            .WithMessage("Mark type is not a valid");
        
        RuleFor(x => x.mark)
            .NotEmpty().WithMessage("Mark is required")
            .Must(mark => Enum.IsDefined(typeof(StudentMarks), mark))
            .WithMessage("Mark is not a valid");
    }
}