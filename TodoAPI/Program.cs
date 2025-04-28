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

// Update Todo
app.MapPut("/todos/{id}", async (int id, Todo updatedTodo, TodoContext db) =>
{
    // Find the Todo to update by its ID
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();  // Return 404 if the Todo doesn't exist

    // Validate the updated Todo object
    var validationContext = new ValidationContext(updatedTodo);
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(updatedTodo, validationContext, validationResults, true))
    {
        return Results.BadRequest(validationResults);  // Return 400 with validation errors
    }

    // Update the Todo's properties
    todo.Title = updatedTodo.Title;
    todo.Description = updatedTodo.Description;
    todo.ExpiryDate = updatedTodo.ExpiryDate;
    todo.PercentComplete = updatedTodo.PercentComplete;

    // Save the changes to the database
    await db.SaveChangesAsync();
    return Results.Ok(todo);  // Return the updated Todo as a 200 response
});


// Get Incoming Todos (Today, Next Day, Current Week)
app.MapGet("/todos/incoming", async (string filter, TodoContext db) =>
{
    var today = DateTime.Today;
    var tomorrow = today.AddDays(1);
    var endOfWeek = today.AddDays(7 - (int)today.DayOfWeek);

    // initial query for all TODOs
    IQueryable<Todo> query = db.Todos;
    
    // Check the filter parameter value
    switch (filter.ToLower())
    {
        case "today":
            query = query.Where(t => t.ExpiryDate.Date == today);
            break;
        case "tomorrow":
            query = query.Where(t => t.ExpiryDate.Date == tomorrow);
            break;
        case "week":
            query = query.Where(t => t.ExpiryDate.Date >= today && t.ExpiryDate.Date <= endOfWeek);
            break;
        default:
            return Results.BadRequest(new { error = "Filter must be one of: today, tomorrow, week" });
    }
    
    // Execute the query and return the list of matching TODOs
    var todos = await query.ToListAsync();
    return Results.Ok(todos); 
});

// Delete Todo
app.MapDelete("/todos/{id}", async (int id, TodoContext db) =>
{
    // Find the Todo by ID
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();  // Return 404 if the Todo doesn't exist

    // Remove the Todo from the database
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.NoContent();  // Return 204 No Content as the Todo is deleted
});

// Set Todo Percent Complete
app.MapPatch("/todos/{id}/percent", async (int id, int percentComplete, TodoContext db) =>
{
    // Find the Todo by ID
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();  // Return 404 if the Todo doesn't exist

    // Validate the percent complete value (must be between 0 and 100)
    if (percentComplete is < 0 or > 100)
        return Results.BadRequest("Percent complete must be between 0 and 100.");

    // Update the Todo's percent complete value
    todo.PercentComplete = percentComplete;
    await db.SaveChangesAsync();

    return Results.Ok(todo);  // Return the updated Todo as a 200 response
});

// Mark Todo as Done 
app.MapPatch("/todos/{id}/done", async (int id, TodoContext db) =>
{
    // Find the Todo by ID
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();  // Return 404 if the Todo doesn't exist

    // Mark the Todo as 100% complete
    todo.PercentComplete = 100;
    await db.SaveChangesAsync();

    return Results.Ok(todo);  // Return the updated Todo as a 200 response
});
app.Run();
