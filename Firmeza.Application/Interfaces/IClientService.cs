using Firmeza.Application.DTOs;
using Firmeza.Application.DTOs.Client;

namespace Firmeza.Application.Interfaces;

public interface IClientService
{
    
    // CRUD
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto dto);
    Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto dto);
    Task<bool> DeleteClientAsync(int id);

    // MÉTRICAS
    Task<int> CountAsync();
    
    Task<int> GetTotalClientsAsync();
    Task<int> GetNewClientsThisMonthAsync();
}