using System.ComponentModel.DataAnnotations;
namespace RetailOrdering.API.DTOs.Brand;

public class CreateBrandDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string BrandName { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}