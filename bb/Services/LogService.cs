using System.Diagnostics;
using bb.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace bb.Services;

public class LogService
{
    private readonly IMongoCollection<Log> _logCollection;
    private readonly IMongoCollection<Project> _projectCollection;

    public LogService(MyDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase("acpe");
        _logCollection = db.GetCollection<Log>("logs");
        _projectCollection = db.GetCollection<Project>("projects");
    }

    public async Task<Log> GetLog(string logId) => await _logCollection.Find(x => x.Id == logId).FirstOrDefaultAsync();

    public async Task AddLog(Log newLog, string projectId)
    {
        await _logCollection.InsertOneAsync(newLog);
        var project = await _projectCollection.Find(x => x.Id == projectId).FirstOrDefaultAsync();
        project.Logs.Add(newLog.Id);
        await _projectCollection.ReplaceOneAsync(x => x.Id == projectId, project);
    }

    public async Task EditLog(Log newLog)
    {
        var filter = Builders<Log>.Filter.Where(_ => _.Id == newLog.Id);
        var update = Builders<Log>.Update.Set(_ => _.Who, newLog.Who)
            .Set(_ => _.Amount, newLog.Amount).Set(_ => _.Purpose, newLog.Purpose);
        var options = new FindOneAndUpdateOptions<Log>();

        await _logCollection.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task<List<LogExtend>?> GetProjectLogs(List<string> logs)
    {
        var logsAsObjectId = new BsonArray();
        foreach (var logId in logs)
        {
            logsAsObjectId.Add(new ObjectId(logId));
        }

        var match = new BsonDocument("$match",
            new BsonDocument("$expr",
                new BsonDocument("$in",
                    new BsonArray
                    {
                        "$_id",
                        logsAsObjectId
                    })));

        var lookup = new BsonDocument("$lookup",
            new BsonDocument
            {
                {"from", "users"},
                {"localField", "Who"},
                {"foreignField", "_id"},
                {"as", "Who"}
            });

        var unwind = new BsonDocument("$unwind",
            new BsonDocument("path", "$Who"));

        var pipeline = new[] {match, lookup, unwind};

        var logsResult = await _logCollection.AggregateAsync<LogExtend>(pipeline);

        var result = logsResult.ToList();
        result.Reverse();

        return result;
    }
}