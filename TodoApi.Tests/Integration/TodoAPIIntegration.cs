using System.Net.Http.Json;
using FluentAssertions;

namespace TodoApi.Tests.Integration;

public class TodoAPIIntegration : IClassFixture<CustomWebApp>
{
    private readonly HttpClient _client;

    public TodoAPIIntegration(CustomWebApp factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTodo_ReturnsCreated()
    {
        var todo = new
        {
            Title = "Test task",
            Description = "Test description",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        var response = await _client.PostAsJsonAsync("/todos", todo);

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoDTO>();
        created.Should().NotBeNull();
        created!.Title.Should().Be(todo.Title);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsList()
    {
        var response = await _client.GetAsync("/todos");

        response.EnsureSuccessStatusCode();
        var todos = await response.Content.ReadFromJsonAsync<List<TodoDTO>>();
        todos.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTodoById_ReturnsTodo()
    {
        var todo = new
        {
            Title = "Single test",
            Description = "Test single description",
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            PercentComplete = 0
        };

        var postResponse = await _client.PostAsJsonAsync("/todos", todo);
        var created = await postResponse.Content.ReadFromJsonAsync<TodoDTO>();

        var getResponse = await _client.GetAsync($"/todos/{created!.Id}");

        getResponse.EnsureSuccessStatusCode();
        var retrieved = await getResponse.Content.ReadFromJsonAsync<TodoDTO>();
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task UpdateTodo_ChangesValues()
    {
        var todo = new
        {
            Title = "To update",
            Description = "Original description",
            ExpiryDate = DateTime.UtcNow.AddDays(3),
            PercentComplete = 0
        };

        var postResponse = await _client.PostAsJsonAsync("/todos", todo);
        var created = await postResponse.Content.ReadFromJsonAsync<TodoDTO>();

        var updatedTodo = new
        {
            Title = "Updated",
            Description = "Updated description",
            ExpiryDate = DateTime.UtcNow.AddDays(5),
            PercentComplete = 50
        };

        var putResponse = await _client.PutAsJsonAsync($"/todos/{created!.Id}", updatedTodo);

        putResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/todos/{created.Id}");
        var retrieved = await getResponse.Content.ReadFromJsonAsync<TodoDTO>();

        retrieved!.Title.Should().Be("Updated");
        retrieved.PercentComplete.Should().Be(50);
    }

    [Fact]
    public async Task DeleteTodo_RemovesTodo()
    {
        var todo = new
        {
            Title = "To delete",
            Description = "To be deleted",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        var postResponse = await _client.PostAsJsonAsync("/todos", todo);
        var created = await postResponse.Content.ReadFromJsonAsync<TodoDTO>();

        var deleteResponse = await _client.DeleteAsync($"/todos/{created!.Id}");

        deleteResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/todos/{created.Id}");

        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}