using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using FluentAssertions;
using Newtonsoft.Json;

namespace TodoApi.Tests;
    

public class TodoApiTest : IClassFixture<TodoAPIFactory>
{
    private readonly HttpClient _client;
    
        public TodoApiTest()
        {
            var appFactory = new WebApplicationFactory<Program>();
            _client = appFactory.CreateClient();
        }
    
        [Fact]
        public async Task GetTodos_ReturnsOkStatus()
        {
            // Act
            var response = await _client.GetAsync("/api/todos");
    
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    
        [Fact]
        public async Task CreateTodo_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var todo = new { }; // empty object
    
            var content = new StringContent(JsonConvert.SerializeObject(todo), Encoding.UTF8, "application/json");
    
            // Act
            var response = await _client.PostAsync("/api/todos", content);
    
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
}