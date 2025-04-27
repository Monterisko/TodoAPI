using Microsoft.EntityFrameworkCore;
using TodoAPI.Context;

var builder = WebApplication.CreateBuilder(args);

// Register the TodoContext with PostgreSQL connection string
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.Urls.Add("http://0.0.0.0:8080");



app.Run();
