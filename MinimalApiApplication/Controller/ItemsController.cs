using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApiApplication.Data;
using MinimalApiApplication.Dtos;
using MinimalApiApplication.Models;
using System.Runtime.InteropServices;

namespace MinimalApiApplication.Controller;

[ApiController]
[Route("[controller]")]
public class ItemsController :ControllerBase
{
    private readonly AppDbContext _dbContext;
    public ItemsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<Item>>>> GetItems()
    {
        var items = await _dbContext.Items.ToListAsync();
        return Ok(new ApiResponse<List<Item>>(items));
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<Item>>> GetItem(int id)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
        {
            return NotFound(new ApiResponse<string>("Item not found"));
        }
        return Ok(new ApiResponse<Item>(item));
    }
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<Item>>>> SearchItems([FromQuery] string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BadRequest(new ApiResponse<string>("Name parameter is required"));
        }

        var filterItems = await _dbContext.Items
            .Where(i => i.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();

        return Ok(new ApiResponse<List<Item>>(filterItems));
    }
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<Item>>> CreateItem([FromBody] Item? item)
    {
        if (item == null || string.IsNullOrEmpty(item.Name) || item.Price <= 0)
        {
            return BadRequest(new ApiResponse<string>("Invalid item data"));
        }
        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ApiResponse<Item>(item));
    }
    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponse<List<Item>>>> CreateItemsBulk([FromBody] List<Item>? items)
    {
        if (items == null || items.Any(i => string.IsNullOrEmpty(i.Name) || i.Price <= 0))
        {
            return BadRequest(new ApiResponse<string>("Invalid item data"));
        }
        _dbContext.Items.AddRange(items);
        await _dbContext.SaveChangesAsync();
        return Ok(new ApiResponse<List<Item>>(items));
    }
    
}