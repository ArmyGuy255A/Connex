using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Services;

public static class ConfigureEntityFramework
{
    public static IServiceCollection AddEntityFrameworkMongoDb(this IServiceCollection services, InAppSettings settings)
    {

        // Add MongoDB services
        if (string.IsNullOrEmpty(settings.MongoDbConnectionString))
        {
            throw new Exception("MongoDb connection string is empty");
        }
        
        services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(settings.MongoDbConnectionString));
        services.AddTransient(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("WebAppDb"));
        services.AddTransient<IMongoCollection<Page>>(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Page>("Pages"));
        return services;
    }
}