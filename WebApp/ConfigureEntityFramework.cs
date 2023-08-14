using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApp.Classes;

namespace WebApp;

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