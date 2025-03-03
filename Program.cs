using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
OrdersRepository repository = new OrdersRepository();
repository.Add(new Order (" ", " ", " ", " " ));
List<string> executors = ["Ivan", "Petr", "Sergey"];
app.MapGet("/", () => "Hello World!");
app.MapGet("orders", () => repository.ReadAll());

app.Run();

public class OrdersRepository : DbContext
{
    private DbSet<Order> Orders { get; set; }
    public OrdersRepository ()
    {
        Orders = Set<Order>();
        Database.EnsureCreated();
    }
    #region CRUD
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data source = orders.db");
    }

    public new void Add (object order)
    {
        if (order is not Order)
            throw new Exception("Wrong data type");
        Orders.Add((Order)order);
    }

    public List<Order> ReadAll()
    {
        return Orders.ToList();
    }

    public Order ReadNumber(int orderNumber)
    {
        return Orders.ToList().Find(order => order.OrderNumber == orderNumber);
    }

    public void Delete(int orderNumber)
    {
        Orders.Remove(ReadNumber(orderNumber));
    }
    #endregion
}


public class Order
{
    [Key]
    public int OrderNumber { get; set; }
    private static int _nextOrderNumber = 1;
    public DateOnly Orderdate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly? ReadyToIssueDate { get; set; }
    public string Device { get; set; }
    public string ProblemType { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.WaitingForExecution;
    public string ClientName { get; set; }
    public string ClientSurname { get; set; }    
    public string? Description { get; set; }
    private string? _executor;
    public string? Executor
    {
        get => _executor;
        set
        {
            _executor = value;
            if (_executor != null)
                Status = OrderStatus.InRepair;
        }
    }

    public Order(string device, string problemType, string clientName, string clientSurname)
    {
        this.OrderNumber = _nextOrderNumber++;
        this.Device = device;
        this.ProblemType = problemType;
        this.ClientName = clientName;
        this.ClientSurname = clientSurname;
    }

    public int EndRepair()
    {
        this.Status = OrderStatus.ReadyToIssue;
        DateOnly ReadyToIssueDate = DateOnly.FromDateTime(DateTime.Now);
        int DaysOfRepair = ReadyToIssueDate.DayNumber - Orderdate.DayNumber;
        return DaysOfRepair;
    }
}

public enum OrderStatus
{
    WaitingForExecution,
    InRepair,
    ReadyToIssue
}