using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PoliceDB.Core.Models;

namespace PoliceDB.DAL
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSettings _settings;

        public MongoDbContext(IOptions<MongoDBSettings> settings)
        {
            _settings = settings.Value;

            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoCollection<MongoUser> Users =>
            _database.GetCollection<MongoUser>(_settings.UsersCollection);

        public IMongoCollection<MongoCase> Cases =>
            _database.GetCollection<MongoCase>(_settings.CasesCollection);

        public IMongoCollection<MongoEvidence> Evidences =>
            _database.GetCollection<MongoEvidence>(_settings.EvidencesCollection);

        public IMongoCollection<MongoPendingChange> PendingChanges =>
            _database.GetCollection<MongoPendingChange>(_settings.PendingChangesCollection);
    }
}