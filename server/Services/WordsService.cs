using MongoDB.Bson;
using MongoDB.Driver;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Services
{
    public class WordsService
    {
        private readonly IMongoCollection<Words> _words;

        public WordsService(IDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _words = database.GetCollection<Words>(settings.WordsCollectionName);
        }

        public Words GetNewWord(int length)
        {
            var pipeline = new BsonDocument[]
            {               
                new BsonDocument { {
                        "$match", new BsonDocument
                        {
                            {
                                "length", length
                            },
                            {
                                "last_used", BsonNull.Value
                            }
                        }
                    } },
                new BsonDocument { {
                        "$sample", new BsonDocument("size", 1)
                    } }
            };

            Words word = _words.Aggregate<Words>(pipeline).FirstOrDefault();
            word.LastUsed = DateTime.Today.ToUniversalTime();

            return word;
        }

        public Words GetWord(string wordToCheck) =>
            _words.Find(word => word.Word == wordToCheck).FirstOrDefault();

        public Words GetWordOfTheDay(int length, BsonDateTime day) =>
            _words.Find(word => word.Length == length && word.LastUsed == day).FirstOrDefault();

        public Words Create(Words word)
        {
            _words.InsertOne(word);
            return word;
        }

        public void Update(string id, Words wordIn) =>
            _words.ReplaceOne(word => word.Id == id, wordIn);

        public void Remove(Words wordIn) =>
            _words.DeleteOne(word => word.Id == wordIn.Id);

        public void Remove(string id) =>
            _words.DeleteOne(word => word.Id == id);
    }
}
