using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TestMongo2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var appConfig = builder.Build().GetSection("AppConfig").Get<AppConfig>();

            var tokenSource = new CancellationTokenSource();
            var client = new MongoClient(appConfig.MongoConnectionString);
            var database = client.GetDatabase(appConfig.MongoDb);
            var collection = database.GetCollection<BsonDocument>(appConfig.CollectionName);
            
            while (true)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(appConfig.FilteredObjectId));

                var res =  collection.Find(filter);
               
                Console.WriteLine($"Read {res.ToList().Count} and wait {appConfig.WaitIntervalSeconds} seconds...");

                WaitHandle.WaitAny(new[] { tokenSource.Token.WaitHandle }, TimeSpan.FromSeconds(appConfig.WaitIntervalSeconds));
            }
        }
    }
}
