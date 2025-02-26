﻿using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Projections.Abstractions
{
    public abstract class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        protected MongoDbContext(IConfiguration configuration)
        {
            MongoUrl mongoUrl = new(configuration.GetConnectionString("Projections"));
            _database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
            => _database.GetCollection<T>(typeof(T).Name);
    }
}
