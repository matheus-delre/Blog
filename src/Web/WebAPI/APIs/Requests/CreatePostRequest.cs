using Application.UseCases.Commands;

namespace WebAPI.APIs.Requests;

public record CreatePostRequest(string Title, string Content)
{
    public static implicit operator CreatePostCommand(CreatePostRequest request)
        => new(new(request.Title), new(request.Content));
}