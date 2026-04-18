using System.ComponentModel.DataAnnotations;

namespace RetailOrdering.API.DTOs.Product;

public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int BrandId { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int InitialStock { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 10;
}