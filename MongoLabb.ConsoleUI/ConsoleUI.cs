using MongoDB.Bson;
using MongoLabb.Data.Interfaces;
using System.Collections;
using System.Net.Http.Json;

namespace MongoLabb.ConsoleUI;

public class ConsoleUI<TProduct> where TProduct : class, IProduct, new()
{
    HttpClient client = new HttpClient();
    string connectionstring = "https://localhost:7181/api/" + typeof(TProduct).Name + "s";
    string productName = typeof(TProduct).Name + "s";

    public async Task Start()
    {
        bool keeprunning = true;
        bool displayError = false;
        do
        {
            Console.Clear();
            Console.Write($"\n" +
                          $"VERY BASIC " + productName.ToUpper() + " UI V 1.0\n" +
                          $"#########################\n" +
                          $"\nMenu\n" +
                          $"--------\n" +
                          $"1. Get all\n" +
                          $"2. Get one by Id\n" +
                          $"3. Get many by propertyname/value\n" +
                          $"4. Add one\n" +
                          $"5. Update one\n" +
                          $"6. Delete one\n" +
                          $"7. Exit\n\n");
            if (displayError)
            {
                displayError = false;
                WriteOutError();
            }
            else
            {
                Console.Write($"\nMenyval: ");
            }
            string? selection = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(selection))
            {
                displayError = true;
            }
            else
            {
                switch (selection)
                {
                    case "1":
                        Console.Clear();
                        Print(await GetAll());
                        Console.ReadKey(true);
                        break;
                    case "2":
                        Console.Clear();
                        Print(await GetById(GetValidId()));
                        Console.ReadKey(true);
                        break;
                    case "3":
                        Console.Clear();
                        Print(await GetByPropNameAndValue(GetValidProp(), GetValidString()));
                        Console.ReadKey(true);
                        break;
                    case "4":
                        Console.Clear();
                        await Add(CreateProduct());
                        Console.ReadKey(true);
                        break;
                    case "5":
                        Console.Clear();
                        await Update(GetValidId(), GetValidProp());
                        Console.ReadKey(true);
                        break;
                    case "6":
                        Console.Clear();
                        await Delete(GetValidId());
                        Console.ReadKey(true);
                        break;
                    case "7":
                        keeprunning = false;
                        client.CancelPendingRequests();
                        client.Dispose();
                        Environment.Exit(0);
                        break;
                    default:
                        displayError = true;
                        break;
                }
            }
        } while (keeprunning);
    }
    async Task<List<TProduct>> GetAll()
    {
        try
        {
            var response = await client.GetAsync(connectionstring);
            if (response is not null && response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TProduct>>();
            }
            else
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return null;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    async Task<List<TProduct>> GetById(string id)
    {
        try
        {
            if (id is not null)
            {
                var response = await client.GetAsync(connectionstring + "/Id?id=" + id);
                if (response is not null && response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<TProduct>();
                    return new List<TProduct> { product };
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return null;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    async Task<List<TProduct>> GetByPropNameAndValue(string propName, string propValue)
    {
        try
        {
            if (propName is not null && propValue is not null)
            {
                var response = await client.GetAsync(connectionstring + $"/PropertyName/PropertyValue?propName={propName}&propValue={propValue}");
                if (response is not null && response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TProduct>>();
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return null;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    async public Task Add(TProduct product)
    {
        try
        {
            if (product is not null)
            {
                var response = await client.PostAsJsonAsync(connectionstring, product);
                if (response is not null && response.IsSuccessStatusCode)
                {
                    var addedProd = await response.Content.ReadFromJsonAsync<TProduct>();
                    Console.WriteLine($"Succesfully added {typeof(TProduct).Name} with Id: {addedProd.Id}");
                }
                else
                {
                    Console.WriteLine($"Could add the {typeof(TProduct).Name}");
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task Update(string id, string propName)
    {
        try
        {
            if (id is not null && propName is not null)
            {
                HttpResponseMessage response = null;

                while (propName == "Id")
                {
                    Console.WriteLine("You can't update the Id.");
                    propName = GetValidProp();
                }
                var prop = typeof(TProduct).GetProperties().ToList().Where(p => p.Name.Equals(propName)).FirstOrDefault();
                if (prop is not null)
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        Console.WriteLine($"What value for: {prop.Name}?");
                        var newPropValue = GetValidString();
                        response = await client.PutAsJsonAsync(connectionstring + $"/Id/PropertyName?id={id}&propName={propName}", newPropValue);
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        Console.WriteLine($"What value for: {prop.Name}?");
                        var newPropValue = GetValidInt();
                        response = await client.PutAsJsonAsync(connectionstring + $"/Id/PropertyName?id={id}&propName={propName}", newPropValue.ToString());
                    }
                    else if (prop.PropertyType == typeof(double))
                    {
                        Console.WriteLine($"What value for: {prop.Name}?");
                        var newPropValue = GetValidDouble();
                        response = await client.PutAsJsonAsync(connectionstring + $"/Id/PropertyName?id={id}&propName={propName}", newPropValue.ToString());
                    }
                    else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                        && prop.PropertyType.GetGenericArguments()[0] == typeof(string))
                    {
                        List<string> list = new();
                        Console.WriteLine($"How many values for: {prop.Name}?");
                        int numberOfValues;
                        do
                        {
                            Console.WriteLine("Amount of values can't be negative.");
                            numberOfValues = GetValidInt();
                        } while (numberOfValues < 0);
                        if (numberOfValues > 0)
                        {
                            for (int i = 0; i < numberOfValues; i++)
                            {
                                Console.WriteLine("Value " + (i + 1));
                                list.Add(GetValidString());
                            }
                        }
                        var newPropValue = list;
                        response = await client.PutAsJsonAsync(connectionstring + $"/Id/PropertyName?id={id}&propName={propName}", newPropValue);
                    }

                }

                if (response is not null && response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Succesfully updated {typeof(TProduct).Name} with Id: {id}");
                }
                else
                {
                    Console.WriteLine($"Could update the {typeof(TProduct).Name} with id: {id}");
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task Delete(string id)
    {
        try
        {
            if (id is not null)
            {
                var response = await client.DeleteAsync(connectionstring + "/Id?id=" + id);
                if (response is not null && response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Succesfully deleted {typeof(TProduct).Name} with Id: {id}");
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private TProduct CreateProduct()
    {
        TProduct newProd = new();
        var propList = typeof(TProduct).GetProperties().ToList();
        foreach (var prop in propList)
        {
            if (prop.Name.Equals("Id"))
            {
                prop.SetValue(newProd, "");
            }
            else if (prop.PropertyType == typeof(string))
            {
                Console.WriteLine($"What value for: {prop.Name}?");
                prop.SetValue(newProd, GetValidString());
            }
            else if (prop.PropertyType == typeof(int))
            {
                Console.WriteLine($"What value for: {prop.Name}?");
                prop.SetValue(newProd, GetValidInt());
            }
            else if (prop.PropertyType == typeof(double))
            {
                Console.WriteLine($"What value for: {prop.Name}?");
                prop.SetValue(newProd, GetValidDouble());
            }
            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                && prop.PropertyType.GetGenericArguments()[0] == typeof(string))
            {
                List<string> list = new();
                Console.WriteLine($"How many values for: {prop.Name}?");
                int numberOfValues;
                do
                {
                    Console.WriteLine("Amount of values can't be negative.");
                    numberOfValues = GetValidInt();
                } while (numberOfValues < 0);
                if (numberOfValues > 0)
                {
                    for (int i = 0; i < numberOfValues; i++)
                    {
                        Console.WriteLine("Value " + (i + 1));
                        list.Add(GetValidString());
                    }
                }
                prop.SetValue(newProd, list);
            }
            else
            {
                prop.SetValue(newProd, null);
            }
        }
        return newProd;
    }

    private string GetValidProp()
    {
        var propList = typeof(TProduct).GetProperties().ToList();
        Console.WriteLine("Available properties to use");
        propList.ForEach(p => Console.WriteLine(propList.IndexOf(p) + 1 + " - " + p.Name));
        Console.WriteLine("Enter number of propertyname to use: ");
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int propNumber))
            {
                if (propNumber <= propList.Count && propNumber > 0)
                {
                    return propList[propNumber - 1].Name;
                }
            }
            Console.WriteLine("That is not a valid number");
        }
    }

    private string GetValidString()
    {
        Console.WriteLine("Enter text: ");
        while (true)
        {
            string? text = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            Console.WriteLine("That is not a valid text input");
        }
    }

    private string GetValidId()
    {
        Console.WriteLine("Enter ID: ");
        while (true)
        {
            string? id = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(id) && id.Length == 24)
            {
                return id;
            }
            Console.WriteLine("That is not a valid ID. 24 chars required.");
        }
    }

    private int GetValidInt()
    {
        while (true)
        {
            Console.WriteLine("Enter number: ");
            if (int.TryParse(Console.ReadLine(), out int number))
            {
                return number;
            }
            Console.WriteLine("That is not a valid number.");
        }
    }

    private double GetValidDouble()
    {
        while (true)
        {
            Console.WriteLine("Enter number: ");
            if (double.TryParse(Console.ReadLine(), out double number) && number > 0)
            {
                return number;
            }
            Console.WriteLine("That is not a valid number.");
        }
    }

    void Print(List<TProduct> products)
    {
        try
        {

            if (products is not null)
            {
                var propList = typeof(TProduct).GetProperties().ToList();
                int maxLength = propList.Max(x => x.Name.Length);
                foreach (var p in products)
                {
                    Console.WriteLine("\n");
                    foreach (var prop in propList)
                    {
                        if (prop.PropertyType.IsArray || prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var values = (IEnumerable)prop.GetValue(p);
                            string concatValues = "";
                            foreach (var item in values)
                            {
                                concatValues += item.ToString() + ", ";
                            }
                            concatValues = concatValues[0..^2];
                            var propName = prop.Name;
                            Console.WriteLine(propName.PadRight(maxLength) + " - " + concatValues);
                        }
                        else
                        {
                            var name = prop.Name;
                            var value = prop.GetValue(p);
                            Console.WriteLine(name.PadRight(maxLength) + " - " + value);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void WriteOutError()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Ogiltigt Menyval! Försök igen");
        Console.ResetColor();
        Console.Write($"Menyval: ");
    }
}
