using courses.Models.DTO;
using courses.Models.enums;
using FluentValidation;

namespace courses.Validators;

public class FilterCoursesValidator : AbstractValidator<CampusCourseFilterModel>
{
    public FilterCoursesValidator()
    {
        RuleFor(x => x.sort)
            .Must(sort => string.IsNullOrEmpty(sort.ToString()) || Enum.TryParse<SortList>(sort.ToString(), out var _))
            .WithMessage("Sort is not a valid");
        
        RuleFor(x => x.hasPlacesAndOpen)
            .Must(hasPlacesAndOpen => hasPlacesAndOpen is null || hasPlacesAndOpen is bool)
            .WithMessage("HasPlacesAndOpen must be a bool");

        RuleFor(x => x.page)
            .NotEmpty().WithMessage("Page is required")
            .GreaterThan(0).WithMessage("Page must be more than 0")
            .Must(page => page is int)
            .WithMessage("Page must be a integer");
        
        RuleFor(x => x.pageSize)
            .NotEmpty().WithMessage("PageSize is required")
            .GreaterThan(0).WithMessage("PageSize must be more than 0")
            .Must(pageSize => pageSize is int)
            .WithMessage("PageSize must be a integer");
    }
}