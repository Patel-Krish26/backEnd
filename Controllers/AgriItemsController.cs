using Microsoft.AspNetCore.Mvc;
using backEnd.Interfaces;
using backEnd.Models;
using backEnd.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace backEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgriItemsController : ControllerBase
{
    private readonly IAgriItemsService _agriItemsService;

    public AgriItemsController(IAgriItemsService agriItemsService)
    {
        _agriItemsService = agriItemsService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var items = await _agriItemsService.GetAllItemsAsync();
        return Ok(items);
    }

    [HttpGet("my-items")]
    [Authorize(Roles = "farmer")]
    public async Task<IActionResult> GetMyItems()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var items = await _agriItemsService.GetItemsByFarmerAsync(email);
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "farmer")]
    public async Task<IActionResult> Create([FromBody] AgriItem item)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        item.PostedBy = email;
        var created = await _agriItemsService.CreateItemAsync(item);
        
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] AgriItemDto dto)
    {
        var updated = await _agriItemsService.UpdateItemAsync(id, dto);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("admin");
        
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var success = await _agriItemsService.DeleteItemAsync(id, email, isAdmin);
        if (!success) return Forbid(); // Or NotFound depending on semantics

        return NoContent();
    }
}
