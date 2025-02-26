using Application.Abstractions;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;

namespace Application.UseCases.Commands;

public record CreatePostCommand(Title Title, Content Content);

public record CreatePostResponse(string PostId, string Title, string Content);

public interface ICreatePostInteractor : IInteractor<CreatePostCommand, CreatePostResponse>;

public class CreatePostInteractor(IApplicationService service) : ICreatePostInteractor
{
    public async Task<CreatePostResponse> InteractAsync(CreatePostCommand cmd, CancellationToken cancellationToken)
    {
        var post = Post.Create(cmd.Title, cmd.Content);

        await service.AppendEventsAsync<Post, PostId>(post, cancellationToken);

        return new(post.Id, post.Title, post.Content);
    }
}