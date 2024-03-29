using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bb.Models;

public class Log
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required] [BsonElement("LogDate")] public DateTime LogDate { get; set; }

    [Required] [BsonElement("Who")] public Guid Who { get; set; }

    [Required] [BsonElement("Amount")] public double Amount { get; set; }

    [Required] [BsonElement("Purpose")] public string Purpose { get; set; }
}