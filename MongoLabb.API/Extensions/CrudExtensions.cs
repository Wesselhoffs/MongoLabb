namespace MongoLabb.API.Extensions;

public static class CrudExtensions 
{
    public static async Task<ActionResult> GetAllAsync<TProduct>(this IDbService dbService) where TProduct : class, IProduct
    {
        try
        {
            var result = await dbService.GetAllProducts<TProduct>();
            if (result is not null && result.Any())
            {
                return new OkObjectResult(result);
            }
            return new NotFoundObjectResult("No products in database");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    public static async Task<ActionResult> GetOneByIdAsync<TProduct>(this IDbService dbService, string id) where TProduct: class, IProduct
    {
        try
        {
            var result = await dbService.GetProductById<TProduct>(id);
            if (result is not null)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundObjectResult($"Product with {id} not found");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    public static async Task<ActionResult> GetByPropnameAndValueAsync<TProduct>(this IDbService dbService, string propName, string propValue) where TProduct : class, IProduct
    {
        var propNames = typeof(TProduct).GetProperties().Select(p => p.Name).ToList();
        if (propNames.Contains(propName))
        {
            try
            {
                var result = await dbService.GetProductByProp<TProduct>(propName, propValue);
                if (result is not null && result.Any())
                {
                    return new OkObjectResult(result);
                }
                return new NotFoundObjectResult($"No products found where {propName} is {propValue}");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        return new BadRequestObjectResult($"{propName} is not a valid property.");
    }

    public static async Task<ActionResult> AddAsync<TProduct>(this IDbService dbService, TProduct product) where TProduct : class, IProduct
    {
        try
        {
            product.Id = "";
            await dbService.AddProduct<TProduct>(product);
            return new CreatedAtActionResult("GetById", typeof(TProduct).Name + "s", new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    public static async Task<ActionResult> UpdateAsync<TProduct>(this IDbService dbService, string id, string propName, string newPropValue) where TProduct : class, IProduct
    {
        var propNames = typeof(TProduct).GetProperties().Select(p => p.Name).ToList();
        if (propNames.Contains(propName))
        {
            try
            {
                if (await dbService.UpdateProduct<TProduct>(id, propName, newPropValue))
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        return new NotFoundObjectResult($"{propName} is not a valid property.");
    }

    public static async Task<ActionResult> DeleteAsync<TProduct>(this IDbService dbService, string id) where TProduct : class, IProduct
    {
        try
        {
            if (await dbService.DeleteProductById<TProduct>(id))
            {
                return new OkResult();
            }
            return new NotFoundObjectResult($"Product with {id} not found");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

}
