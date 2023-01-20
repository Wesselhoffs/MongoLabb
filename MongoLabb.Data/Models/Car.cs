namespace MongoLabb.Data.Models;

public class Car : ProductBase, IProduct
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
}
