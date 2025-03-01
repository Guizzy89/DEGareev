var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Order order1 = new (1, " ", " ", " ", " " );
List<string> executors = ["Ivan", "Petr", "Sergey"];
app.MapGet("/", () => "Hello World!");
app.MapGet("orders", () => order1);

app.Run();

public class Order
{
    public Order(int orderNumber, string device, string problemType, string clientName, string clientSurname)
    {
        this.OrderNumber = orderNumber;
        this.Device = device;
        this.ProblemType = problemType;
        this.ClientName = clientName;
        this.ClientSurname = clientSurname;
    }

    public int OrderNumber { get; set; }
    public DateOnly Orderdate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
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
}

public enum OrderStatus
{
    WaitingForExecution,
    InRepair,
    ReadyToIssue
}