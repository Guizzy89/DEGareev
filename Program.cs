var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Order order1 = new (1, " ", " ", " ", " ", " " ); 

app.MapGet("/", () => "Hello World!");
app.MapGet("orders", () => order1);

app.Run();

public class Order
{
    public Order(int OrderNumber, string Device, string ProblemType, string ClientName, string ClientSurname, string Executor)
    {
    }

    public int OrderNumber { get; set; }
    public DateOnly Orderdate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string Device { get; set; }
    public string ProblemType { get; set; }
    public string? Description { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.WaitingForExecution;
    public string ClientName { get; set; }
    public string ClientSurname { get; set; }
    public string? Executor { get; set; }
}

public enum OrderStatus
{
    WaitingForExecution,
    InRepair,
    ReadyToIssue
}