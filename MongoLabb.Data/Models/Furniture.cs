namespace MongoLabb.Data.Models
{
    public class Furniture : ProductBase, IProduct
    {
        public string Type { get; set; }
        public string Color { get; set; }
    }
}
