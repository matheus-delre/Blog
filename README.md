## :bulb: About

The main objective of this **cloud-native** project is to represent the state-of-the-art of a **distributed**, **reliable**, and **highly scalable** system by interpreting the most relevant principles of [**Reactive
Domain Driven Design**](https://www.infoq.com/articles/modeling-uncertainty-reactive-ddd/).

> Domain-Driven Design can aid with managing uncertainty through the use of good modeling.  
> -- Vaughn Vernon

**Scalability** and **Resilience** require **low coupling** and **high cohesion**, principles strongly linked to the proper understanding of the business through **well-defined boundaries**, combined with a healthy and
efficient integration strategy such as **Event-driven Architecture** (EDA).

The [**Event Storming**](https://www.eventstorming.com/) workshop provides a practical approach to **subdomain decomposition**, using **Pivotal Events** to correlate business capabilities across **Bounded Contexts** while promoting **reactive
integration** between Aggregates.

The **reactive** integration between **Bounded Contexts** configures an **Event-driven Architecture** (EDA) where **Commands** are acknowledged and sent to the Bus by the Web API (BFF/Gateway) while **Events** are
broadcasted to the **Query** side and/or other Aggregates.

**Independence**, as the main characteristic of a **Microservice**, can only be found in a **Bounded Context**.

The [**Event Sourcing**](https://www.eventstore.com/event-sourcing) is a proprietary implementation that, in addition to being **naturally auditable** and **data-driven**, represents the most efficient persistence mechanism ever. An **eventual state
transition** Aggregate design is essential at this point. The **Event Store** comprises EF Core (ORM) + MSSQL (Database).

> State transitions are an important part of our problem space and should be modeled within our domain.  
> -- Greg Young

[**Projections**](https://www.eventstore.com/event-sourcing#Projections) are **asynchronously denormalized** and stored on a NoSQL Database(MongoDB); Nested documents should be avoided here; Each projection has its index and **fits perfectly into a view or component**,
mitigating unnecessary data traffic and making the reading side as efficient as possible.

The splitting between **Command** and **Query** stacks occurs logically through the [**CQRS**](https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf) pattern and fiscally via a **Microservices** architecture. Each stack is an individual deployable unit with its database,
and the data flows from Command to Query stack via **Domain** and/or **Summary** events.

As a **domain-centric** approach, [**Clean Architecture**](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) provides the appropriate isolation between the **Core** (Application + Domain) and "times many" **Infrastructure** concerns.

### Project Structure

#### Domain Layer

The Domain namespace contains the core domain logic for managing posts. Key classes and concepts in this layer
include:

* **Post**: An aggregate root representing a post with properties like title, content, and comments.
* **DomainEvent**: A set of domain events related to posts, such as **PostCreated** and **CommentAdded**.
* Entity used within the domain, such as **Comment**.
* Value objects used within the domain, such as **Title** and **Content**.

#### Application Layer

The Application namespace contains the application layer responsible for handling user interactions and orchestrating
domain actions. Key classes and concepts in this layer include:

* **CreatePostCommand**: A command to create a new post, specifying title and content.
* **CreatePostResponse**: A response containing information about the created post, including postId, title,
  and content.
* **CreateCommentCommand**: A command to create a comment, specifying postId and content.
* **ProjectPostWhenPostChangedInteractor**: An interactor for handling events when a post's changed.
* **ProjectCommentWhenCommentChangedInteractor**: An interactor for handling events when a comment's changed.
* **IGetPostByIdInteractor**: An interface for retrieving a post by its ID, returning the fields postId, title, content, and comments.
* **IListPostInteractor**: An interface for retrieving a post by its ID, returning the fields postId, title, content, and the number of comments.
* **IApplicationService**: An interface defining application-level services for loading aggregates and appending events.

#### Infrastructure Layer

The infrastructure layer is responsible for the technical concerns that support the domain and application layers. It
provides implementations for the interfaces declared in the application layer.

##### Event Bus Gateway

The **EventBusGateway** class provides the specific implementation for the **IEventBusGateway** that is declared in the
application layer. It is responsible for publishing domain events. The class
leverages MassTransit, a free, open-source distributed application framework for .NET, to make the message routing more
manageable and decoupled.

* The **PublishAsync** method publishes events immediately.

This separation of message routing into an infrastructure service facilitates the testing of application layer
use-cases, as you can replace the actual event bus with a mock implementation for unit testing.

##### Event Store Gateway

Event sourcing persisting strategy is implemented through the **EventStoreGateway** class, that provides an implementation
for the **IEventStoreGateway** contract.

* The **AppendAsync** method appends new events to the event store, which in this case is a DbContext that can represent a
relational database.

* The **GetStreamAsync** method retrieves a stream (sequence) of events for a specific aggregate instance, identified by id,
from the stored event stream.

This persistent storage for the domain events permits replaying events to reconstruct the state of an aggregate root, a
significant advantage for troubleshooting and auditing purposes. But also, the event store acts as a write-side storage
model in the **Command Query Responsibility Segregation** (CQRS) pattern.

##### Projection Gateway

The event sourcing query strategy is implemented through the **ProjectionGateway** class, which provides an implementation 
for the **IProjectionGateway** contract.

* The **FindAsync** method retrieves a specific projection that matches a given predicate,
allowing flexible querying based on domain-specific conditions.

* The **GetAsync** method fetches a projection by its unique identifier, 
ensuring efficient access to read models without reconstructing the event stream.

* The **ListAsync** method returns a list of all stored projections, 
enabling bulk retrieval of read models for reporting and querying purposes.

* The **ReplaceInsertAsync** method updates an existing projection or inserts a new one, 
ensuring the read model remains synchronized with the latest domain events.

* The **GetCollection** method provides direct access to the underlying MongoDB collection, 
allowing advanced querying and indexing strategies when necessary.

Projections serve as a crucial component in **Event Sourcing**, enabling optimized and performant read-side models in the 
**Command Query Responsibility Segregation (CQRS)** pattern. By maintaining precomputed views of domain data, 
projections allow fast, scalable queries while keeping the write-side event store separate from the read-side representation.

# :computer: Running

[Docker](https://www.docker.com/why-docker/) makes it easy to set up that closely mirrors the production environment without having to install and configure all of the dependencies on your local
machine. Especially useful for working on complex applications that rely on many different libraries and tools.

#### Docker

The respective [./docker-compose.yaml](./docker-compose.yaml) will provide all the necessary resources, with public exposure to the connection
ports:

```bash
docker-compose -f ./docker-compose.yaml up -d
```

If prefer, is possible to use individual Docker commands:

MSSQL

```bash
docker run -d \
-e 'ACCEPT_EULA=Y' \
-e 'SA_PASSWORD=!MyStrongPassword' \
-p 1433:1433 \
--name mssql \
mcr.microsoft.com/mssql/server
```

MongoDB

```bash
docker run -d \
-e 'MONGO_INITDB_ROOT_USERNAME=mongoadmin' \
-e 'MONGO_INITDB_ROOT_PASSWORD=secret' \
-p 27017:27017 \
--name mongodb \
mongo
```

RabbitMQ

```bash
docker run -d \
-p 15672:15672 \
-p 5672:5672 \
--hostname my-rabbit \
--name rabbitmq \
rabbitmq:3-management
```

## :test_tube: Tests

<details>
    <summary>Unit Tests</summary>

### Unit Tests

To unit-test an event-sourced aggregate, it's to verify that the Aggregate produces the expected event as output given a specific set of input Events and a Command. This involves creating an Aggregate
instance, applying the input events to it, handling the command, and verifying the expected event output.

```csharp
[Fact]
public void CreatePost_ShouldRaise_PostCreated()
    => Given()
        .When(Post.Create(_title, _content))
        .Then<DomainEvent.PostCreated>(
            @event => @event.PostId.Should().NotBe(PostId.Undefined),
            @event => @event.Title.Should().Be(_title),
            @event => @event.Content.Should().Be(_content),
            @event => @event.Version.Should().Be(Version.Initial));
```

</details>
