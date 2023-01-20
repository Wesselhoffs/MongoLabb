namespace MongoLabb.Data.Models;

public class Book : ProductBase, IProduct
{
    public string AuthorFirstName { get; set; }
    public string AuthorLastName { get; set; }
    public List<string> Genres { get; set; }
}
