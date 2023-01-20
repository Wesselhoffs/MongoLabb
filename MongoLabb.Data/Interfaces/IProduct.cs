namespace MongoLabb.Data.Interfaces;

public interface IProduct
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonRequired]
    [BsonElement("Name")]
    public string Name { get; set; }
    [BsonRequired]
    [BsonElement("Description")]
    public string Description { get; set; }
    [BsonRequired]
    [BsonElement("Price")]
    public double Price { get; set; }
    [BsonRequired]
    [BsonElement("Balance")]
    public int Balance { get; set; }
}
