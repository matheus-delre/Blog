using AutoFixture;
using BlogTests.Abstractions;
using Domain.Abstractions.Messages;
using Domain.Aggregates;
using Domain.Aggregates.Events;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using Version = Domain.ValueObjects.Version;

namespace BlogTests
{
    public class PostUnitTests : AggregateTests<Post, PostId>
    {
        private static readonly Fixture Fixture = new();

        private readonly PostId _postId = PostId.New;
        private readonly CommentId _commentId = CommentId.New;
        private readonly Title _title = Fixture.Create<Title>();
        private readonly Content _content = Fixture.Create<Content>();

        [Fact]
        public void CreatePost_ShouldRaise_PostCreated()
            => Given()
                .When(Post.Create(_title, _content))
                .Then<DomainEvent.PostCreated>(
                    @event => @event.PostId.Should().NotBe(PostId.Undefined),
                    @event => @event.Title.Should().Be(_title),
                    @event => @event.Content.Should().Be(_content),
                    @event => @event.Version.Should().Be(Version.Initial));

        [Fact]
        public void AddComment_ShouldRaise_CommentAdded()
        {
            Given(new DomainEvent.PostCreated(_postId, _title, _content, Version.Initial))
                .When(post => post.AddComment(_content))
                .Then<DomainEvent.CommentAdded>(
                    @event => @event.CommentId.Should().NotBe(CommentId.Undefined),
                    @event => @event.PostId.Should().NotBe(PostId.Undefined),
                    @event => @event.Content.Should().Be(_content),
                    @event => @event.Version.Should().Be(Version.Number(2)));
        }
    }
}
