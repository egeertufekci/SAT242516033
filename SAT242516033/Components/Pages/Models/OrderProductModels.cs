namespace SAT242516033.Components.Pages.Models;

public class ProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }

    // optional navigation for UI
    public ProductDto? Product { get; set; }
}