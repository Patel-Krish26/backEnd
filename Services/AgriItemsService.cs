using backEnd.Interfaces;
using backEnd.Models;
using backEnd.DTOs;
using backEnd.Repositories;
using AutoMapper;

namespace backEnd.Services;

public class AgriItemsService : IAgriItemsService
{
    private readonly IAgriItemsRepository _repo;
    private readonly IMapper _mapper;

    public AgriItemsService(IAgriItemsRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AgriItemDto>> GetAllItemsAsync()
    {
        var items = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<AgriItemDto>>(items);
    }

    public async Task<IEnumerable<AgriItemDto>> GetItemsByFarmerAsync(string email)
    {
        var items = await _repo.GetByFarmerEmailAsync(email);
        return _mapper.Map<IEnumerable<AgriItemDto>>(items);
    }

    public async Task<AgriItem> CreateItemAsync(AgriItem item)
        => await _repo.CreateAsync(item);

    public async Task<AgriItemDto?> UpdateItemAsync(long id, AgriItemDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        await _repo.UpdateAsync(existing);

        return _mapper.Map<AgriItemDto>(existing);
    }

    public async Task<bool> DeleteItemAsync(long id, string userEmail, bool isAdmin)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        if (item.PostedBy != userEmail && !isAdmin) return false;

        await _repo.DeleteAsync(item);
        return true;
    }
}
