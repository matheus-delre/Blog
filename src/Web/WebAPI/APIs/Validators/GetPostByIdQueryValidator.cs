using Application.UseCases.Queries;
using Domain.Aggregates;
using FluentValidation;

namespace WebAPI.APIs.Validators
{

    public class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
    {
        public GetPostByIdQueryValidator()
        {
            RuleFor(x => x.PostId)
                .NotNull()
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _)).WithMessage("PostId must be a valid GUID")
                .Must(id => id != PostId.Undefined);
        }
    }
}
