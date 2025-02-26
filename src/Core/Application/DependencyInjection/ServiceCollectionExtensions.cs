using Application.Services;
using Application.UseCases.Commands;
using Application.UseCases.Events;
using Application.UseCases.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddApplicationServices()
            .AddCommandInteractors()
            .AddEventInteractors()
            .AddQueryInteractors();

    private static IServiceCollection AddCommandInteractors(this IServiceCollection services)
        => services
            .AddScoped<ICreatePostInteractor, CreatePostInteractor>()
            .AddScoped<ICreateCommentInteractor, CreateCommentInteractor>();

    private static IServiceCollection AddEventInteractors(this IServiceCollection services)
        => services
            .AddScoped<IProjectCommentWhenCommentChangedInteractor, ProjectCommentWhenCommentChangedInteractor>()
            .AddScoped<IProjectPostWhenPostChangedInteractor, ProjectPostWhenPostChangedInteractor>();

    private static IServiceCollection AddQueryInteractors(this IServiceCollection services)
        => services
            .AddScoped<IListPostInteractor, ListPostInteractor>()
            .AddScoped<IGetPostByIdInteractor, GetPostByIdInteractor>();

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        => services
            .AddScoped<IApplicationService, ApplicationService>();
}