using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace bb.Models;

[CollectionName("roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{
    
}