using backEnd.Data;
using backEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace backEnd.Repositories;

public class AgriItemsRepository : IAgriItemsRepository
{
    private readonly AppDbContext _context;

    public AgriItemsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AgriItem>> GetAllAsync()
        => await _context.AgriItems.ToListAsync();

    public async Task<IEnumerable<AgriItem>> GetByFarmerEmailAsync(string email)
        => await _context.AgriItems.Where(i => i.PostedBy == email).ToListAsync();

    public async Task<AgriItem?> GetByIdAsync(long id)
        => await _context.AgriItems.FindAsync(id);

    public async Task<AgriItem> CreateAsync(AgriItem item)
    {
        _context.AgriItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<AgriItem> UpdateAsync(AgriItem item)
    {
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteAsync(AgriItem item)
    {
        _context.AgriItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}
