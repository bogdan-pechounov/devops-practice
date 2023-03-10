using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
  public static class RepositoryExtensions
  {
    public static void AddMongo(this IServiceCollection services)
    {

      services.AddSingleton(serviceProvider =>
      {
        var configuration = serviceProvider.GetService<IConfiguration>();
        var serviceSetings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
        var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
        var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
        return mongoClient.GetDatabase(serviceSetings.ServiceName);
      });
    }

    public static void AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
      services.AddSingleton<IRepository<T>>(serviceProvider =>
         {
           var database = serviceProvider.GetService<IMongoDatabase>();
           return new MongoRepository<T>(database, collectionName);
         });
    }
  }
}