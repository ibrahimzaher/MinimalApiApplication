namespace MinimalApiApplication.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
