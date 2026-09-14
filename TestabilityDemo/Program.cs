namespace TestabilityDemo;

public class Program
{
    public static void Main(string[] args)
    {
        // This is NOT how you should be coding!
        var db = new Database();
        var now = DateTime.Now;
        var order = new Order(now);

        Console.WriteLine("Vad vill du köpa?");
        order.Item = Console.ReadLine();
        order.Price = db.GetItemPrice(order.Item);

        Console.WriteLine("Hur många?");
        order.Quantity = float.Parse(Console.ReadLine());


        Console.WriteLine($"""
                   ********************************
                   KVITTO
                   {order.Item}({order.Price}) x {order.Quantity}
                   rabatt: {order.Discount}%
                   Total price: {order.GetTotal()}:-
                   ~~~~~~~~~~~~~~~~~~~~~ 
                   {now}
                   ********************************
                   """);


        // Problems 
        // 1. All the code is in main
        // 2. Implicit Dependency on DateTime.Now
        // 3. Implicit Dependency on Database
        // 4. Console.WriteLine
        // 5. Price and printing logic is mixed


    }
    public class Database
    {
        // Let's pretend this actually connects to a database
        public float GetItemPrice(string id) => new Random().Next(1, 10);
    }

}
