using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Entities;
using Play.Common.Repositories;
using Play.Common.Settings;

namespace Play.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            return configuration.GetSection("MongoDBSettingJSON").Get<MongoDbSettings>()
                ?? throw new InvalidOperationException("MongoDB settings are missing.");
        });
        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            return configuration.GetSection("ServiceSettingJSON").Get<ServiceSettings>()
                ?? throw new InvalidOperationException("Service settings are missing.");
        });
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var mongoDbSettings = serviceProvider.GetRequiredService<MongoDbSettings>();
            return new MongoClient(mongoDbSettings.ConnectionString);
        });
        services.AddSingleton(serviceProvider =>
        {
            var serviceSettings = serviceProvider.GetRequiredService<ServiceSettings>();
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(
        this IServiceCollection services,
        string collectionName) where T : class, IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
            ActivatorUtilities.CreateInstance<MongoRepository<T>>(serviceProvider, collectionName));

        return services;
    }
}
