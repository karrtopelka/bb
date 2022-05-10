using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bb.Models;

public class LogExtend
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required] [BsonElement("LogDate")] public DateTime LogDate { get; set; }

    [Required] [BsonElement("Who")] public ApplicationUser Who { get; set; }

    [Required] [BsonElement("Amount")] public double Amount { get; set; }

    [Required] [BsonElement("Purpose")] public string Purpose { get; set; }
}