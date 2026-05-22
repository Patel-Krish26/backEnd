using backEnd.DTOs;
using backEnd.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backEnd.Interfaces;

public interface IAgriItemsService
{
    Task<IEnumerable<AgriItemDto>> GetAllItemsAsync();
    Task<IEnumerable<AgriItemDto>> GetItemsByFarmerAsync(string email);
    Task<AgriItem> CreateItemAsync(AgriItem item);
    Task<AgriItemDto?> UpdateItemAsync(long id, AgriItemDto dto);
    Task<bool> DeleteItemAsync(long id, string userEmail, bool isAdmin);
}
