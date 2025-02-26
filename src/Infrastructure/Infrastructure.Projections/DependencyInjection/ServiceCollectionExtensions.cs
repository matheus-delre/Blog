﻿using Application.Abstractions.Gateways;
using Infrastructure.Projections.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Projections.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjections(this IServiceCollection services)
        {
            services.AddScoped(typeof(IProjectionGateway<>), typeof(ProjectionGateway<>));
            services.AddScoped<IMongoDbContext, ProjectionDbContext>();

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            return services;
        }
    }
}
