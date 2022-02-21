namespace server.Models
{

    public class WordDatabaseSettings : IDbSettings
    {
        public string WordsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDbSettings
    {
        string WordsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}