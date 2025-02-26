using Domain.Abstractions.EventStore;
using Domain.Abstractions.Identities;
using Domain.Aggregates;
using Infrastructure.EventStore.Contexts.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Version = Domain.ValueObjects.Version;

namespace Infrastructure.EventStore.Contexts.Configurations;

public class StoreEventConfiguration : IEntityTypeConfiguration<StoreEvent<Post, PostId>>
{
    public void Configure(EntityTypeBuilder<StoreEvent<Post, PostId>> builder)
    {
        builder.ToTable($"{nameof(Post)}StoreEvents");

        builder.HasKey(@event => new { @event.Version, @event.AggregateId });
        builder
        .Property(@event => @event.AggregateId)
            .HasConversion<Guid>(id => id, guid => GuidIdentifier.New<PostId>(guid))
            .IsRequired();

        builder
            .Property(@event => @event.Event)
            .HasConversion<EventConverter>()
            .IsRequired();

        builder
            .Property(@event => @event.EventType)
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder
            .Property(@event => @event.Timestamp)
            .IsRequired();

        builder
            .Property(@event => @event.Version)
            .HasConversion<ushort>(version => version, number => Version.Number(number))
            .IsRequired();
    }
}