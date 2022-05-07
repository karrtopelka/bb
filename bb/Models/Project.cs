using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bb.Models;

public class Project
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    [BsonElement("ProjectName")]
    public string ProjectName { get; set; }

    [Required]
    [BsonElement("ProjectStatus")]
    public bool ProjectStatus { get; set; }

    [BsonElement("ProjectDescription")] public string? ProjectDescription { get; set; }

    [Required] [BsonElement("Author")] public Guid Author { get; set; }

    [Required]
    [BsonElement("DateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("DateEnded")] public DateTime? DateEnded { get; set; }

    [BsonElement("Participants")] public List<Guid> Participants { get; set; }

    [BsonElement("Logs")] public List<Guid> Logs { get; set; }
}