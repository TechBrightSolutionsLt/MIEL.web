using System.ComponentModel.DataAnnotations;

public class ImageItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ImagePath { get; set; }
}