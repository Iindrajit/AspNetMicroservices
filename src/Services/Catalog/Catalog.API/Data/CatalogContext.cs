using Catalog.API.Entities;
using Catalog.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IOptions<CatalogDatabaseSettings> connectionSettings)
        {
            var client = new MongoClient(connectionSettings.Value.ConnectionString);
            var database = client.GetDatabase(connectionSettings.Value.DatabaseName);
            Products = database.GetCollection<Product>(connectionSettings.Value.CollectionName);

            CatalogContextSeed.SeedData(Products);
        }
        public IMongoCollection<Product> Products { get; }
    }
}
