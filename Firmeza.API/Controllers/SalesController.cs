using AutoMapper;
using Firmeza.Application.DTOs.Sale;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
    private readonly IProductRepository _productRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesController> _logger;
    private readonly IEmailService _emailService;

    public SalesController(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<SalesController> logger,
        IEmailService emailService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
        _emailService = emailService;
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
            // Get current user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario");
            }
            
            // Filter sales by current user
            var allSales = await _saleRepository.GetAllAsync();
            var userSales = allSales.Where(s => s.UserId == userId);
            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(userSales);
            
            _logger.LogInformation("User {UserId} retrieved {Count} sales", userId, userSales.Count());
            
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

            // Validate user exists
            var user = await _userManager.FindByIdAsync(createDto.UserId);
            if (user == null)
            {
                return BadRequest($"Usuario con ID {createDto.UserId} no encontrado");
            }

            // Create sale entity
            var sale = new Sale
            {
                UserId = createDto.UserId,
                SaleDate = DateTime.UtcNow,
                SaleDetails = new List<SaleDetail>()
            };

            decimal totalAmount = 0;
            
            // Build email content for order details
            var orderDetailsBuilder = new System.Text.StringBuilder();
            orderDetailsBuilder.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
            orderDetailsBuilder.AppendLine("<tr style='background-color: #f2f2f2;'><th>Producto</th><th>Cant.</th><th>Precio</th><th>Total</th></tr>");

            // Process each item in the sale
            foreach (var itemDto in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    return BadRequest($"Producto con ID {itemDto.ProductId} no encontrado");
                }

                // Validate stock availability
                if (product.Stock < itemDto.Quantity)
                {
                    return BadRequest($"Stock insuficiente para {product.Name}. Disponible: {product.Stock}, Solicitado: {itemDto.Quantity}");
                }

                // Create sale detail
                var saleDetail = new SaleDetail
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                sale.SaleDetails.Add(saleDetail);
                totalAmount += saleDetail.Total;

                // Update product stock
                product.Stock -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);

                // Add item details to email content
                orderDetailsBuilder.AppendLine($"<tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'>{product.Name}</td><td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: center;'>{itemDto.Quantity}</td><td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>${product.Price:N2}</td><td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>${saleDetail.Total:N2}</td></tr>");
            }

            sale.TotalAmount = totalAmount;
            await _saleRepository.AddAsync(sale);

            // Finalize email content with total amount
            orderDetailsBuilder.AppendLine($"<tr style='font-weight: bold;'><td colspan='3' style='padding: 8px; text-align: right;'>Total:</td><td style='padding: 8px; text-align: right;'>${totalAmount:N2}</td></tr>");
            orderDetailsBuilder.AppendLine("</table>");

            var saleDto = _mapper.Map<SaleDto>(sale);
            
            _logger.LogInformation("Sale created: {SaleId} for user {UserId}, Total: {TotalAmount}", sale.Id, sale.UserId, sale.TotalAmount);
            
            // Send purchase confirmation email asynchronously (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendPurchaseConfirmationAsync(user.Email!, orderDetailsBuilder.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send purchase confirmation email to {Email}", user.Email);
                }
            });

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
