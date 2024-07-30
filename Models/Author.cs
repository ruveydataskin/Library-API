using LibraryAPI6.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Author
{
    public long Id { get; set; }

    [Required]
    [StringLength(800)]
    public string FullName { get; set; } = "";

    [Column(TypeName = "nvarchar(1000)")]
    public string? Biography { get; set; }

    [Range(-4000, 2100)]
    public short BirthYear { get; set; }

    [Range(-4000, 2100)]
    public short? DeathYear { get; set; }
}