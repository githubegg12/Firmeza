using AutoMapper;
using Firmeza.Application.DTOs.Sale;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.API.Controllers;

/// <summary>
/// Controller for sales management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesController> _logger;

    public SalesController(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<SalesController> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all sales (Admin only)
    /// </summary>
    /// <returns>List of all sales</returns>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(IEnumerable<SaleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll()
    {
        try
        {
            var sales = await _saleRepository.GetAllAsync();
            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
            return Ok(saleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all sales");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Get sale by ID
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>Sale details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleDto>> GetById(int id)
    {
        try
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            var saleDto = _mapper.Map<SaleDto>(sale);
            return Ok(saleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sale {SaleId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Get current user's sales (Cliente role)
    /// </summary>
    /// <returns>List of user's sales</returns>
    [HttpGet("my-sales")]
    [Authorize(Roles = "Cliente")]
    [ProducesResponseType(typeof(IEnumerable<SaleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetMySales()
    {
        try
        {
            // In a real implementation, you would filter by the current user's client ID
            // For now, return all sales (this should be enhanced with user-client relationship)
            var sales = await _saleRepository.GetAllAsync();
            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
            
            _logger.LogInformation("User {User} retrieved their sales", User.Identity?.Name);
            
            return Ok(saleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user sales");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    /// <param name="createDto">Sale creation data</param>
    /// <returns>Created sale</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaleDto>> Create([FromBody] CreateSaleDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sale = _mapper.Map<Sale>(createDto);
            sale.SaleDate = DateTime.UtcNow;
            
            await _saleRepository.AddAsync(sale);

            var saleDto = _mapper.Map<SaleDto>(sale);
            
            _logger.LogInformation("Sale created: {SaleId} for client {ClientId}", sale.Id, sale.ClientId);
            
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, saleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Update an existing sale (Admin only)
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="updateDto">Sale update data</param>
    /// <returns>Updated sale</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaleDto>> Update(int id, [FromBody] UpdateSaleDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingSale = await _saleRepository.GetByIdAsync(id);
            if (existingSale == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            _mapper.Map(updateDto, existingSale);
            await _saleRepository.UpdateAsync(existingSale);

            var saleDto = _mapper.Map<SaleDto>(existingSale);
            
            _logger.LogInformation("Sale updated: {SaleId}", id);
            
            return Ok(saleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sale {SaleId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Delete a sale (Admin only)
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            await _saleRepository.DeleteAsync(sale);
            
            _logger.LogInformation("Sale deleted: {SaleId}", id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sale {SaleId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
