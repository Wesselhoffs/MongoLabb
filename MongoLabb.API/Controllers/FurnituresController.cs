namespace MongoLabb.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FurnituresController : ControllerBase
{
    private readonly IDbService DbService;

    public FurnituresController(MongoDbService dbService)
    {
        DbService = dbService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        return await DbService.GetAllAsync<Furniture>();
    }

    [HttpGet("Id")]
    public async Task<ActionResult> GetById(string id)
    {
        return await DbService.GetOneByIdAsync<Furniture>(id);
    }

    [HttpGet("PropertyName/PropertyValue")]
    public async Task<ActionResult> GetByValues(string propName, string propValue)
    {
        return await DbService.GetByPropnameAndValueAsync<Furniture>(propName, propValue);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Furniture furniture)
    {
        return await DbService.AddAsync<Furniture>(furniture);
    }

    [HttpPut("Id/PropertyName")]
    public async Task<ActionResult> Put(string id, string propName, [FromBody] string newPropValue)
    {
        return await DbService.UpdateAsync<Furniture>(id, propName, newPropValue);
    }

    [HttpDelete("Id")]
    public async Task<ActionResult> Delete(string id)
    {
        return await DbService.DeleteAsync<Furniture>(id);
    }
}
