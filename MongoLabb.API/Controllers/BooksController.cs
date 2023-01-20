namespace MongoLabb.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IDbService DbService;

    public BooksController(MongoDbService dbService)
    {
        DbService = dbService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        return await DbService.GetAllAsync<Book>();
    }

    [HttpGet("Id")]
    public async Task<ActionResult> GetById(string id)
    {
        return await DbService.GetOneByIdAsync<Book>(id);
    }

    [HttpGet("PropertyName/PropertyValue")]
    public async Task<ActionResult> GetByValues(string propName, string propValue)
    {
        return await DbService.GetByPropnameAndValueAsync<Book>(propName, propValue);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Book book)
    {
        return await DbService.AddAsync<Book>(book);
    }

    [HttpPut("Id/PropertyName")]
    public async Task<ActionResult> Put(string id, string propName, [FromBody] string newPropValue)
    {
        return await DbService.UpdateAsync<Book>(id, propName, newPropValue);
    }

    [HttpDelete("Id")]
    public async Task<ActionResult> Delete(string id)
    {
        return await DbService.DeleteAsync<Book>(id);
    }
}
