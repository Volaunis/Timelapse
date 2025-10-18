using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Timelapse.Models;
using Timelapse.Repositories.Configuration;

namespace Timelapse.Repositories.Services;

public interface IMongoDbService
{
    public List<T> GetList<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>>? sortField = null, bool? ascending = null) where T : MongoModel;

    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> filter) where T : MongoModel;
    public T Get<T>(string id) where T : MongoModel;
    public T? Get<T>(Expression<Func<T, bool>> filter) where T : MongoModel;
    public T Insert<T>(T document) where T : MongoModel;
    public void Replace<T>(T document) where T : MongoModel;
    public void Update<T>(T document, Dictionary<Expression<Func<T, object>>, object> updateFields) where T : MongoModel;
    public void Delete<T>(string id) where T : MongoModel;
}

public class MongoDbService : IMongoDbService
{
    private readonly MongoDbConfiguration _mongoDbConfiguration;
    private readonly MongoClient _client;

    public MongoDbService(IOptions<MongoDbConfiguration> mongoDbConfiguration)
    {
        _mongoDbConfiguration = mongoDbConfiguration.Value;

        _client = new MongoClient(_mongoDbConfiguration.ConnectionString);
    }

    public List<T> GetList<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>>? sortField = null, bool? ascending = null) where T : MongoModel
    {
        var collection = GetCollection<T>();

        var findFluent = collection.Find(filter);

        if (sortField != null && ascending != null)
        {
            findFluent = ascending.Value
                ? findFluent.SortBy(sortField)
                : findFluent.SortByDescending(sortField);
        }

        return findFluent.ToList();
    }

    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> filter) where T : MongoModel
    {
        var collection = GetCollection<T>();

        return await collection.Find(filter).ToListAsync();
    }

    public T Get<T>(string id) where T : MongoModel
    {
        var collection = GetCollection<T>();
        return collection.Find(x => x.Id == new ObjectId(id)).Single();
    }

    public T? Get<T>(Expression<Func<T, bool>> filter) where T : MongoModel
    {
        var collection = GetCollection<T>();
        return collection.Find(filter).SingleOrDefault();
    }

    public T Insert<T>(T document) where T : MongoModel
    {
        var collection = GetCollection<T>();
        collection.InsertOne(document);
        return document;
    }

    public void Replace<T>(T document) where T : MongoModel
    {
        var collection = GetCollection<T>();
        collection.ReplaceOne(x => x.Id == document.Id, document);
    }

    public void Update<T>(T document, Dictionary<Expression<Func<T, object>>, object> updateFields) where T : MongoModel
    {
        var collection = GetCollection<T>();
        var update = Builders<T>.Update;
        var updateDefinitions = updateFields.Select(field => update.Set(field.Key, field.Value)).ToList();
        collection.UpdateOne(x => x.Id == document.Id, update.Combine(updateDefinitions));
    }

    public void Delete<T>(string id) where T : MongoModel
    {
        var collection = GetCollection<T>();
        collection.DeleteOne(x => x.Id == new ObjectId(id));
    }

    private IMongoCollection<T> GetCollection<T>() where T : class
    {
        return _client
            .GetDatabase(_mongoDbConfiguration.Database)
            .GetCollection<T>(typeof(T).Name);
    }
}