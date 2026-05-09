using Microsoft.EntityFrameworkCore;
using MinimalApiApplication.Data;
using MinimalApiApplication.Middleware;
using MinimalApiApplication.Models;
using MinimalApiApplication.Dtos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(
    options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
app.UseStatusCodePages();
app.UseExceptionHandler();
app.UseMiddleware<IPBlockingMiddleware>();
var items=app.MapGroup("/items");

items.MapGet("/", async (AppDbContext db) =>
    Results.Ok(new ApiResponse<List<Item>>(await db.Items.ToListAsync())));

items.MapGet("/{id:int}", async (AppDbContext db, int id) =>
    await db.Items.FirstOrDefaultAsync(i => i.Id == id) is Item item
    ? Results.Ok(new ApiResponse<Item>(item)) 
    : Results.NotFound(new ApiResponse<string>("Item not found"))
);

items.MapGet("/search",async (AppDbContext db,string? name) =>
{
    if(string.IsNullOrEmpty(name)) 
        return Results.BadRequest(new ApiResponse<string>("Name parameter is required"));
    var filterItems=await db.Items
        .Where(i => i.Name.ToLower().Contains(name.ToLower()))
        .ToListAsync();
    return Results.Ok(new ApiResponse<List<Item>>(filterItems));
});
items.MapPost("/", async (AppDbContext db, Item? item) => {
   
    if(item is null || string.IsNullOrEmpty(item.Name) || item.Price <= 0)
    {
        return Results.BadRequest(new ApiResponse<string>("Invalid item data"));
    }

    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", new ApiResponse<Item>(item));
});
items.MapPost("/bulk", async (AppDbContext db, List<Item>? items) => {

    if (items is null || items.Any(i => string.IsNullOrEmpty(i.Name) || i.Price <= 0))
    {
        return Results.BadRequest(new ApiResponse<string>("Invalid item data"));
    }

    db.Items.AddRange(items);
    await db.SaveChangesAsync();
    return Results.Ok(new ApiResponse<List<Item>>(items));
});

app.MapGet("/error", async (context) => throw (new Exception("Error Occur")));
app.Run();
