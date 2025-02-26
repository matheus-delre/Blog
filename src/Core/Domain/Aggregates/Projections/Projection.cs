namespace Domain.Aggregates.Projections
{
    public static class Projection
    {
        public record Post(string Id, string Title, string Content) : IProjection;

        public record Comment(string Id, string PostId, string Content) : IProjection;
    }
}
