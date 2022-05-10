using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bb.Models;

public class ProjectExtend
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

    [Required] [BsonElement("Author")] public ApplicationUser? Author { get; set; }

    [Required]
    [BsonElement("DateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("DateEnded")] public DateTime? DateEnded { get; set; }

    [BsonElement("Participants")] public List<ApplicationUser>? Participants { get; set; }

    [BsonElement("Logs")] public List<LogExtend>? Logs { get; set; }
}