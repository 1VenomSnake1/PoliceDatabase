namespace PoliceDB.Core.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; set; } = "PoliceDatabase";
        public string UsersCollection { get; set; } = "Users";
        public string CasesCollection { get; set; } = "Cases";
        public string EvidencesCollection { get; set; } = "Evidences";
        public string PendingChangesCollection { get; set; } = "PendingChanges";
    }
}