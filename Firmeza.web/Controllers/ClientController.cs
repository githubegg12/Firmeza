using Firmeza.Domain.Entities;
using Firmeza.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Firmeza.Application.Features.Email.Interfaces;

namespace Firmeza.web.Controllers;

[Authorize(Roles = "Administrador")]
public class ClientController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ClientController> _logger;
    private readonly IEmailService _emailService;

    public ClientController(UserManager<ApplicationUser> userManager, ILogger<ClientController> logger, IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
    }

    // GET: Client
    // Retrieves a list of all users with the "Cliente" role
    public async Task<IActionResult> Index()
    {
        var clients = await _userManager.GetUsersInRoleAsync("Cliente");
        return View(clients.ToList());
    }

    // GET: Client/Create
    // Displays the form to create a new client
    public IActionResult Create()
    {
        return View();
    }

    // POST: Client/Create
    // Handles the creation of a new client, including password assignment and welcome email
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClientViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}", // Combine first and last names
                DocumentId = model.DocumentId,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                EmailConfirmed = true // Auto-confirm email since admin created it
            };

            // Create user with the provided password
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Assign "Cliente" role to the new user
                await _userManager.AddToRoleAsync(user, "Cliente");
                
                // Send welcome email asynchronously (fire and forget)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
                    }
                });

                TempData["Success"] = "Cliente creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }


    // GET: Client/Details/5
    // Displays details of a specific client
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var client = await _userManager.FindByIdAsync(id);
        if (client == null)
            return NotFound();

        // Ensure the user is actually a client
        var roles = await _userManager.GetRolesAsync(client);
        if (!roles.Contains("Cliente"))
            return NotFound();

        return View(client);
    }

    // GET: Client/Edit/5
    // Displays the form to edit an existing client
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var client = await _userManager.FindByIdAsync(id);
        if (client == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(client);
        if (!roles.Contains("Cliente"))
            return NotFound();

        return View(client);
    }

    // POST: Client/Edit/5
    // Handles the updates to an existing client's information
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,FullName,Email,DocumentId,PhoneNumber,Address")] ApplicationUser client)
    {
        if (id != client.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound();

                // Update user properties
                user.FullName = client.FullName;
                user.Email = client.Email;
                user.UserName = client.Email; // Keep username synced with email
                user.DocumentId = client.DocumentId;
                user.PhoneNumber = client.PhoneNumber;
                user.Address = client.Address;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Cliente actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClientExists(client.Id))
                    return NotFound();
                else
                    throw;
            }
        }
        return View(client);
    }

    // GET: Client/Delete/5
    // Displays the confirmation page for deleting a client
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var client = await _userManager.FindByIdAsync(id);
        if (client == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(client);
        if (!roles.Contains("Cliente"))
            return NotFound();

        return View(client);
    }

    // POST: Client/Delete/5
    // Confirms and executes the deletion of a client
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var client = await _userManager.FindByIdAsync(id);
        if (client != null)
        {
            var result = await _userManager.DeleteAsync(client);
            if (result.Succeeded)
            {
                TempData["Success"] = "Cliente eliminado exitosamente.";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el cliente.";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // Helper method to check if a client exists and has the correct role
    private async Task<bool> ClientExists(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;
        
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Cliente");
    }
}
