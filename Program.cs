using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ConsoleAppDop2HomeworkCore;

 class Program

{
    static void Main()
    {
        var builder = new ConfigurationBuilder();
    builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
    var options = optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection")).Options;
        using (ApplicationContext db = new ApplicationContext(options))
        {
            var result = from c in db.Clients
                         join o in db.Orders on c.ClientId equals o.ClientId
                         join od in db.OrderDetails on o.OrderId equals od.OrderId
                         group new { o, od } by c into g
                         select new
                         {
                             Name = g.Key.Name,
                             Email = g.Key.Email,
                             Address = g.Key.Address,
                             TotalOrders = g.Count(),
                             TotalAmountSpent = g.Sum(x => x.od.Quantity * x.od.Price),
                             MostExpensiveItem = g.Max(x => x.od.Price)
                         };

            foreach (var item in result)
            {
                Console.WriteLine($"Name: {item.Name}, Email: {item.Email}, Address: {item.Address}");
                Console.WriteLine($"Total Orders: {item.TotalOrders}, Total Amount Spent: {item.TotalAmountSpent.ToString("F2")}");
                Console.WriteLine($"Most Expensive Item: {item.TotalAmountSpent.ToString("F2")}\n");
            }
        }
    }
}

public class ApplicationContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }

    public ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
       
    }
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }

    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

}


    