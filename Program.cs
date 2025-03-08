using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
var app = builder.Build();
OrdersRepository repository = new OrdersRepository();
List<string> executors = ["Ivan", "Petr", "Sergey"];
app.MapGet("/", () => "Hello World!");
app.UseCors(option =>
option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGet("orders", async () => await repository.ReadAll());
app.MapGet("/orders/{orderNumber:int}", async (int orderNumber) =>
{
    try
    {
        var order = await Task.FromResult(repository.ReadNumber(orderNumber));
        return Results.Ok(order);
    }
    catch (ArgumentException ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.MapPost("/orders/add", async ([FromBody] Order order) =>
{
    await repository.Add(order);
    return Results.Created($"/orders/{order.OrderNumber}", order);
});

app.MapDelete("/orders/{orderNumber:int}", async (int orderNumber) =>
{
    await repository.Delete(orderNumber);
    return Results.NoContent();
});

app.MapPut("/orders/{orderNumber}/description", async (int orderNumber, [FromBody] string description) =>
{
    var order = await repository.ReadNumber(orderNumber);
    order.Description = description;
    await repository.SaveChangesAsync();
    return Results.Ok(order);
});

app.Run();

public class OrdersRepository : DbContext
{
    private DbSet<Order> Orders { get; set; }
    public OrdersRepository ()
    {
        Orders = Set<Order>();
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data source = orders.db");
    }
    #region CRUD
    public async Task Add(Order order)
    {
        try
        {
            Orders.Add(order);
            await SaveChangesAsync(); 
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Ошибка при добавлении заказа: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Order>> ReadAll()
    {
        return await Orders.ToListAsync();
    }

    public async Task<Order> ReadNumber(int orderNumber)
    {
        var order = await Orders.FirstOrDefaultAsync(order => order.OrderNumber == orderNumber);
        if (order == null) throw new ArgumentException($"Order with number {orderNumber} not found.");      

        return order;
    }

    public async Task Delete(int orderNumber)
    {
        var order = await ReadNumber(orderNumber);
        Orders.Remove(order);
        await SaveChangesAsync();
    }
    #endregion
}

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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