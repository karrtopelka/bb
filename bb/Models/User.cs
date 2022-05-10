using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bb.Models;
[BsonIgnoreExtraElements]
public class User
{
    public Guid _id { get; set; }
    [Required] [BsonElement("UserName")] public string UserName { get; set; }


    [Required]
    [BsonElement("Email")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; }

    [Required] [BsonElement("Password")] public string Password { get; set; }
}