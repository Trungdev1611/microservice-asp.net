using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public interface IItemRepository
{
    Task CreateAsync(Item entity);
    Task<IReadOnlyCollection<Item>> GetAllAsync();
    Task<Item> GetItemAsync(Guid id);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Item entity);
}

public class ItemRepository : IItemRepository
{
    private const string collectionName = "items";
    private readonly IMongoCollection<Item> dbCollection;

    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    public ItemRepository(IMongoDatabase database)
    {

        dbCollection = database.GetCollection<Item>(collectionName);

    }

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
    {
        return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<Item> GetItemAsync(Guid id)
    {
        FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Item entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Item entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        FilterDefinition<Item> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await dbCollection.ReplaceOneAsync(filter, entity);

    }

    public async Task RemoveAsync(Guid id)
    {
        FilterDefinition<Item> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, id);
        await dbCollection.DeleteOneAsync(filter);

    }
}