using backEnd.Models;
using backEnd.DTOs;

namespace backEnd.Repositories;

public interface IAgriItemsRepository
{
    Task<IEnumerable<AgriItem>> GetAllAsync();
    Task<IEnumerable<AgriItem>> GetByFarmerEmailAsync(string email);
    Task<AgriItem?> GetByIdAsync(long id);
    Task<AgriItem> CreateAsync(AgriItem item);
    Task<AgriItem> UpdateAsync(AgriItem item);
    Task DeleteAsync(AgriItem item);
}
