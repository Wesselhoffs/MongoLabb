using MongoLabb.Data.Interfaces;

namespace MongoLabb.Data.Models;

public class MongoDbService : IDbService 
{
    private readonly MongoClient Client;
    private readonly IMongoDatabase Database;

    public MongoDbService(IOptions<ProductsDatabaseSettings> dbSettings)
    {
        Client = new MongoClient(dbSettings.Value.ConnectionString);
        Database = Client.GetDatabase(dbSettings.Value.DatabaseName);
    }
    public async Task<List<TProduct>> GetAllProducts<TProduct>() where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        var result = await prodCollection.Find(p => true).ToListAsync();
        if (result is not null && result.Any()) return result;
        return null;
    }

    public async Task<TProduct> GetProductById<TProduct>(string id) where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        var result = await prodCollection.FindAsync(p => p.Id == id);
        return await result.FirstOrDefaultAsync();
    }

    public async Task<List<TProduct>> GetProductByProp<TProduct>(string propName, string propValue) where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        var filter = Builders<TProduct>.Filter.Eq(propName, propValue);
        var result = await prodCollection.FindAsync(filter);
        return await result.ToListAsync();
    }

    public async Task AddProduct<TProduct>(TProduct product) where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        await prodCollection.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProduct<TProduct>(string id, string propName, string newPropValue) where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        var filter = Builders<TProduct>.Filter.Eq("Id", id);
        var update = Builders<TProduct>.Update.Set(propName, newPropValue);
        var result = await prodCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount != 0;
    }

    public async Task<bool> ReplaceProduct<TProduct>(string id, TProduct product) where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        var filter = Builders<TProduct>.Filter.Eq("Id", id);
        product.Id = id;
        var result = await prodCollection.ReplaceOneAsync(filter, product);
        return result.ModifiedCount != 0;
    }

    public async Task<bool> DeleteProductById<TProduct>(string id) where TProduct : class, IProduct
    {
        var prodCollection = GetCollection<TProduct>();
        var filter = Builders<TProduct>.Filter.Eq("Id", id);
        var result = await prodCollection.DeleteOneAsync(filter);
        return result.DeletedCount != 0;
    }

    private IMongoCollection<TProduct> GetCollection<TProduct>() where TProduct : class, IProduct
    {
        return Database.GetCollection<TProduct>(typeof(TProduct).Name + "s");
    }

}
