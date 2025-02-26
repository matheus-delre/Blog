using FluentValidation;
using WebAPI.APIs.Requests;

namespace WebAPI.APIs.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotNull()
            .NotEmpty()
            .Length(5, 240)
            .WithMessage("Comment must be between 10 and 240 characters long.");
    }
}