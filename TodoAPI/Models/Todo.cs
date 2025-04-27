namespace TodoAPI.Models;

using System.ComponentModel.DataAnnotations;


public class Todo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters.")]
    public string Title { get; set; }

    [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "ExpiryDate is required.")]
    public DateTime ExpiryDate { get; set; }

    [Range(0, 100, ErrorMessage = "PercentComplete must be between 0 and 100.")]
    public int PercentComplete { get; set; }
}