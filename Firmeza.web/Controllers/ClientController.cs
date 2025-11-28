using Firmeza.Domain.Entities;
using Firmeza.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.web.Controllers;

[Authorize(Roles = "Administrador")]
public class ClientController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ClientController> _logger;

    public ClientController(UserManager<ApplicationUser> userManager, ILogger<ClientController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Client
    public async Task<IActionResult> Index()
    {
        var clients = await _userManager.GetUsersInRoleAsync("Cliente");
        return View(clients.ToList());
    }

    // GET: Client/Details/5
    public async Task<IActionResult> Details(string? id)
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

    // GET: Client/Edit/5
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

    private async Task<bool> ClientExists(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;
        
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Cliente");
    }
}
