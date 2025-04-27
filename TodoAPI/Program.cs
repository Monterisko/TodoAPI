using Microsoft.EntityFrameworkCore;
using TodoAPI.Context;

var builder = WebApplication.CreateBuilder(args);

// Register the TodoContext with PostgreSQL connection string
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();



// Get All Todos
app.MapGet("/todos", async (TodoContext db) =>
{
    // Fetch all Todo items from the database
    var todos = await db.Todos.ToListAsync();
    return Results.Ok(todos);  // Return the list of todos as an HTTP 200 response
});

app.Run();
