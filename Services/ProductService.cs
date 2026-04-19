using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Product;
using RetailOrdering.API.DTOs.Brand;
using RetailOrdering.API.DTOs.Category;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IInventoryRepository _inventoryRepo;

    public ProductService(IProductRepository productRepo, IInventoryRepository inventoryRepo)
    {
        _productRepo = productRepo;
        _inventoryRepo = inventoryRepo;
    }

    public async Task<PagedResult<ProductDto>> GetAllAsync(ProductFilterDto filter)
    {
        var (items, totalCount) = await _productRepo.GetAllAsync(filter);
        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<ProductDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int productId)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            ImageUrl = dto.ImageUrl,
            IsActive = dto.IsActive
        };

        var created = await _productRepo.CreateAsync(product);

        // Initialise inventory record
        await _inventoryRepo.CreateAsync(new Inventory
        {
            ProductId = created.ProductId,
            QuantityInStock = dto.InitialStock,
            ReorderLevel = dto.ReorderLevel
        });

        var full = await _productRepo.GetByIdAsync(created.ProductId);
        return MapToDto(full!);
    }

    public async Task<ProductDto?> UpdateAsync(int productId, CreateProductDto dto)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;

        await _productRepo.UpdateAsync(product);
        var full = await _productRepo.GetByIdAsync(productId);
        return MapToDto(full!);
    }

    public async Task<bool> DeleteAsync(int productId)
        => await _productRepo.DeleteAsync(productId);

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        var cats = await _productRepo.GetAllCategoriesAsync();
        return cats.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description
        });
    }

    public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
    {
        var brands = await _productRepo.GetAllBrandsAsync();
        return brands.Select(b => new BrandDto
        {
            BrandId = b.BrandId,
            BrandName = b.BrandName,
            Description = b.Description
        });
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        ProductId = p.ProductId,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.CategoryName ?? string.Empty,
        BrandId = p.BrandId,
        BrandName = p.Brand?.BrandName ?? string.Empty,
        ImageUrl = p.ImageUrl,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        QuantityInStock = p.Inventory?.QuantityInStock ?? 0
    };
}