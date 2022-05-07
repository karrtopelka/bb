using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
namespace bb.Models;

[CollectionName("users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    
}