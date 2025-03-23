using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FlaschenpostApi.Entities;

public class TodoItem
{
    public void Update(TodoItem other)
    {
        var properties = other.GetType().GetProperties();
        foreach (var property in properties)
        {
            // do not overwrite id
            if (property.Name.Equals(nameof(Id)))
                continue;

            property.SetValue(this, property.GetValue(other));
        }
    }

    public int Id { get; set; }

    [Required, MinLength(2), MaxLength(100)]
    public string? Title { get; set; }

    [Required]
    public DateTime? IssuedDateTime { get; set; }

    [Required]
    public DateTime? DueDateTime { get; set; }

    public DateTime? LastUpdatedDateTime { get; set; }
    public DateTime? CompletedDateTime { get; set; }

    [Required, MinLength(10), MaxLength(500)]
    public string? Description { get; set; }
}