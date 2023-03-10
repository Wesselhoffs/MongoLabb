namespace MongoLabb.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
    private readonly IDbService DbService;

    public CarsController(MongoDbService dbService)
    {
        DbService = dbService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        return await DbService.GetAllAsync<Car>();
    }

    [HttpGet("Id")]
    public async Task<ActionResult> GetById(string id)
    {
        return await DbService.GetOneByIdAsync<Car>(id);
    }

    [HttpGet("PropertyName/PropertyValue")]
    public async Task<ActionResult> GetByValues(string propName, string propValue)
    {
        return await DbService.GetByPropnameAndValueAsync<Car>(propName, propValue);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Car car)
    {
        return await DbService.AddAsync<Car>(car);
    }

    [HttpPut("Id/PropertyName")]
    public async Task<ActionResult> Put(string id, string propName, [FromBody] string newPropValue)
    {
        return await DbService.UpdateAsync<Car>(id, propName, newPropValue);
    }

    [HttpDelete("Id")]
    public async Task<ActionResult> Delete(string id)
    {
        return await DbService.DeleteAsync<Car>(id);
    }
}
