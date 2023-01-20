using MongoLabb.ConsoleUI;
using MongoLabb.Data.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ConsoleUI<Book> ui = new ConsoleUI<Book>();

        ConsoleUI<Car> car = new ConsoleUI<Car>();

        ConsoleUI<Furniture> furniture= new ConsoleUI<Furniture>();

        //await furniture.Start();

        await car.Start();
        //await ui.Start();
    }
}