using Application.UseCases.Commands;
using Application.UseCases.Queries;
using Domain.Aggregates;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.APIs.Requests;
using WebAPI.APIs.Validators;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace WebAPI.APIs;

public static class PostApi
{
    private const string BaseUrl = "/api/v1/posts/";

    public static void MapPostsApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BaseUrl);

        group.MapPost("/", async (ICreatePostInteractor interactor, CreatePostRequest request, CancellationToken cancellationToken) =>
        {
            var result = await new CreatePostRequestValidator().ValidateAsync(request, cancellationToken);
            return result.IsValid ? await CreatePostAsync() : ValidationProblem(result.ToDictionary());

            async Task<Results<Created<CreatePostResponse>, ValidationProblem>> CreatePostAsync()
            {
                var createdPost = await interactor.InteractAsync(request, cancellationToken);

                return Created($"/api/v1/posts/{createdPost.PostId}", createdPost);
            }
        });

        group.MapPost("/{postId:guid}/comments", async (ICreateCommentInteractor interactor, Guid postId, CreateCommentRequest request, CancellationToken cancellationToken) =>
        {
            var result = await new CreateCommentRequestValidator().ValidateAsync(request, cancellationToken);
            return result.IsValid ? await CreateCommentAsync() : ValidationProblem(result.ToDictionary());

            async Task<Results<NoContent, ValidationProblem>> CreateCommentAsync()
            {
                await interactor.InteractAsync(new CreateCommentCommand(new(postId.ToString()), new(request.Content)), cancellationToken);

                return NoContent();
            }
        });

        group.MapGet("/", async (IListPostInteractor interactor, CancellationToken cancellationToken) =>
        {
            var response = await interactor.InteractAsync(new ListPostQuery(), cancellationToken);

            return response.Count != 0
                ? Results.Ok(response)
                : Results.NoContent();
        });

        group.MapGet("/{postId}/", async (string postId, IGetPostByIdInteractor interactor, CancellationToken cancellationToken) =>
        {
            var query = new GetPostByIdQuery((PostId)postId);
            var result = await new GetPostByIdQueryValidator().ValidateAsync(query, cancellationToken);

            return result.IsValid ? await GetPostByIdAsync() : ValidationProblem(result.ToDictionary());

            async Task<Results<Ok<GetPostByIdResponse>, NotFound, ValidationProblem>> GetPostByIdAsync()
            {
                var response = await interactor.InteractAsync(query, cancellationToken);

                return response is not  null ? TypedResults.Ok(response) : TypedResults.NotFound();
            }
        });

    }
}