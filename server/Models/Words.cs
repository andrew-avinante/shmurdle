using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace server.Models
{
    public class Words
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("word")]
        public string Word { get; set; }
        [BsonElement("length")]
        public int Length { get; set; }
        [BsonElement("last_used")]
        public DateTime? LastUsed { get; set; }
    }
}
