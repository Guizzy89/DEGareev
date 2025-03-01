using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Order order1 = new (" ", " ", " ", " " );
List<string> executors = ["Ivan", "Petr", "Sergey"];
app.MapGet("/", () => "Hello World!");
app.MapGet("orders", () => order1);

app.Run();

public class Order
{
    private static int _nextOrderNumber = 1;
    public int OrderNumber { get; set; }
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