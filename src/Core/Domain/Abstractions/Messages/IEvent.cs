﻿using MassTransit;

namespace Domain.Abstractions.Messages;

[ExcludeFromTopology]
public interface IEvent : IMessage;

[ExcludeFromTopology]
public interface IDelayedEvent : IEvent;

[ExcludeFromTopology]
public interface IVersionedEvent : IEvent
{
    ulong Version { get; }
}

[ExcludeFromTopology]
public interface IDomainEvent : IVersionedEvent;
