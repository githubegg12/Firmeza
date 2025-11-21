using AutoMapper;
using Firmeza.Application.DTOs;
using Firmeza.Application.DTOs.Client;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.API.Controllers;

/// <summary>
/// Controller for client management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(
        IClientRepository clientRepository,
        IMapper mapper,
        ILogger<ClientsController> logger)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all clients (Admin only)
    /// </summary>
    /// <returns>List of all clients</returns>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return Ok(clientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all clients");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Get client by ID
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>Client details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            var clientDto = _mapper.Map<ClientDto>(client);
            return Ok(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client {ClientId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Create a new client
    /// </summary>
    /// <param name="createDto">Client creation data</param>
    /// <returns>Created client</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = _mapper.Map<Client>(createDto);
            await _clientRepository.AddAsync(client);

            var clientDto = _mapper.Map<ClientDto>(client);
            
            _logger.LogInformation("Client created: {ClientId} - {ClientName}", client.Id, client.Name);
            
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Update an existing client
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <param name="updateDto">Client update data</param>
    /// <returns>Updated client</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> Update(int id, [FromBody] UpdateClientDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingClient = await _clientRepository.GetByIdAsync(id);
            if (existingClient == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            _mapper.Map(updateDto, existingClient);
            await _clientRepository.UpdateAsync(existingClient);

            var clientDto = _mapper.Map<ClientDto>(existingClient);
            
            _logger.LogInformation("Client updated: {ClientId} - {ClientName}", id, existingClient.Name);
            
            return Ok(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {ClientId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Delete a client (Admin only)
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            await _clientRepository.DeleteAsync(client);
            
            _logger.LogInformation("Client deleted: {ClientId} - {ClientName}", id, client.Name);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {ClientId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
