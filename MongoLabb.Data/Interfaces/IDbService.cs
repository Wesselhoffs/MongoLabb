namespace MongoLabb.Data.Interfaces;

public interface IDbService
{
    public Task<List<TProduct>> GetAllProducts<TProduct>() where TProduct : class, IProduct;
    public Task<TProduct> GetProductById<TProduct>(string id) where TProduct : class, IProduct;
    public Task<List<TProduct>> GetProductByProp<TProduct>(string propName, string propValue) where TProduct : class, IProduct;
    public Task AddProduct<TProduct>(TProduct product) where TProduct : class, IProduct;
    public Task<bool> UpdateProduct<TProduct>(string id, string fieldName, string newFieldValue) where TProduct : class, IProduct;
    public Task<bool> ReplaceProduct<TProduct>(string id, TProduct product) where TProduct : class, IProduct;
    public Task<bool> DeleteProductById<TProduct>(string id) where TProduct : class, IProduct;
}
