using bb.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace bb.Services;

public class DocumentService
{
    private const string DbName = "acpe";
    private const string UsersCollection = "users";
    private readonly MongoClient _client;
    private readonly IMongoDatabase _db;
    private Dictionary<string, List<string>> _databasesAndCollections;

    public DocumentService(MyDatabaseSettings settings)
    {
        _client = new MongoClient(settings.ConnectionString);
        _db = _client.GetDatabase(DbName);
    }
    
    public async Task<Dictionary<string, List<string>>> GetDatabasesAndCollections()
    {
        if (_databasesAndCollections != null) return _databasesAndCollections;

        _databasesAndCollections = new Dictionary<string, List<string>>();
        var databasesResult = await _client.ListDatabaseNamesAsync();

        await databasesResult.ForEachAsync(async databaseName =>
        {
            var collectionNames = new List<string>();
            var database = _client.GetDatabase(databaseName);
            var collectionNamesResult = await database.ListCollectionNamesAsync();
            await collectionNamesResult.ForEachAsync(
                collectionName => { collectionNames.Add(collectionName); });
            _databasesAndCollections.Add(databaseName, collectionNames);
        });

        return _databasesAndCollections;
    }
    
    public async Task<BsonDocument> GetDocument(string collectionName, int index)
    {
        var collection = GetCollection(collectionName);
        BsonDocument document = null;
        await collection.Find(doc => true)
            .Skip(index)
            .Limit(1)
            .ForEachAsync(doc => document = doc);
        return document;
    }

    public async Task<long> GetCollectionCount(string collectionName)
    {
        var collection = GetCollection(collectionName);
        return await collection.EstimatedDocumentCountAsync();
    }

    private IMongoCollection<BsonDocument> GetCollection(string collectionName)
    {
        var db = _client.GetDatabase(DbName);
        return db.GetCollection<BsonDocument>(collectionName);
    }
    
    public async Task<UpdateResult> CreateOrUpdateField(string collectionName, string id, string fieldName, string value)
    {
        var collection = GetCollection(collectionName);
        var update = Builders<BsonDocument>.Update.Set(fieldName, new BsonString(value));
        return await collection.UpdateOneAsync(CreateIdFilter(id), update);
    }

    public async Task<DeleteResult> DeleteDocument(string databaseName, string collectionName, string id)
    {
        var collection = GetCollection(collectionName);
        return await collection.DeleteOneAsync(CreateIdFilter(id));
    }

    private static BsonDocument CreateIdFilter(string id)
    {
        return new BsonDocument("_id", new BsonObjectId(new ObjectId(id)));
    }

    public async Task CreateDocument(string databaseName, string collectionName)
    {
        var collection = GetCollection(collectionName);
        await collection.InsertOneAsync(new BsonDocument());
    }

    public async Task CreateUser(User user)
    {
        var coll = _db.GetCollection<User>(UsersCollection);
        await coll.InsertOneAsync(user);
    }
}