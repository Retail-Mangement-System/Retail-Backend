namespace RetailOrdering.API.DTOs.Inventory;

public class InventoryDto
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsLowStock => QuantityInStock <= ReorderLevel;
}

public class UpdateInventoryDto
{
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
}

public class AdjustStockDto
{
    public int Adjustment { get; set; }   // positive = add, negative = deduct
    public string Reason { get; set; } = string.Empty;
}