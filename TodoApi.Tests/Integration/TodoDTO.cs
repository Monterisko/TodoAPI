namespace TodoApi.Tests.Integration;

public class TodoDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime ExpiryDate { get; set; }
    public int PercentComplete { get; set; }
}