using FluentValidation;
using WebAPI.APIs.Requests;

namespace WebAPI.APIs.Validators;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull()
            .NotEmpty()
            .Length(5, 50)
            .WithMessage("Title must be between 5 and 50 characters long.");

        RuleFor(x => x.Content)
            .NotNull()
            .NotEmpty()
            .Length(5, 240)
            .WithMessage("Content must be between 10 and 240 characters long.");
    }
}