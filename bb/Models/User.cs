using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bb.Models;

public class User
{
    public Guid _id { get; set; }
    [Required] [BsonElement("username")] public string UserName { get; set; }

    [Required]
    [BsonElement("email")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; }

    [Required] [BsonElement("password")] public string Password { get; set; }
}