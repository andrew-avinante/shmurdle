using MongoDB.Bson;
using MongoDB.Driver;
using server.Models;
using System;
using System.IO;

namespace server.Services
{
    public class WordImporter
    {
        private readonly IMongoCollection<BsonDocument> _words;

        public WordImporter(IDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _words = database.GetCollection<BsonDocument>(settings.WordsCollectionName);
        }

        public bool IsCollectionEmpty()
        {
            var count = _words.CountDocuments(FilterDefinition<BsonDocument>.Empty);
            return count == 0;
        }

        public void ImportWords(string filePath)
        {
            if (!IsCollectionEmpty())
            {
                Console.WriteLine("Collection is not empty. Skipping import.");
                return;
            }

            using (var fileStream = File.OpenText(filePath))
            {
                string line;
                while ((line = fileStream.ReadLine()) != null)
                {
                    string word = line.Trim();
                    int length = word.Length;
                    if (length >= 3 && length <= 10)
                    {
                        var document = new BsonDocument
                    {
                        { "word", word },
                        { "length", length },
                        { "last_used", BsonNull.Value }
                    };
                        _words.InsertOne(document);
                    }
                }
            }
        }
    }
}
