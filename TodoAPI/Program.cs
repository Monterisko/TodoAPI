using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Context;
using TodoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Register the TodoContext with PostgresSQL connection string
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

// Create Todo
app.MapPost("/todos", async (Todo todo, TodoContext db) =>
{
    // Validate the provided Todo object using Data Annotations
    var validationContext = new ValidationContext(todo);
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(todo, validationContext, validationResults, true))
    {
        return Results.BadRequest(validationResults);  // Return 400 with validation errors
    }

    // Add the new Todo to the database
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);  // Return 201 with the created Todo
});

// Get Specific Todo
app.MapGet("/todos/{id}", async (int id, TodoContext db) =>
{
    // Find a specific Todo by its ID
    var todo = await db.Todos.FindAsync(id);
    return todo != null ? Results.Ok(todo) : Results.NotFound();  // Return 200 if found, or 404 if not
});
app.Run();
