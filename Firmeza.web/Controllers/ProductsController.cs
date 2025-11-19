using Firmeza.Application.Features.Products.Commands;
using Firmeza.Application.Features.Products.Queries;
using Firmeza.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Features.Products.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICreateProductCommand _createCommand;
        private readonly IGetProductsQuery _getProductsQuery;
        private readonly IUpdateProductCommand _updateCommand;
        private readonly IDeleteProductCommand _deleteCommand;

        public ProductsController(
            ICreateProductCommand createCommand,
            IGetProductsQuery getProductsQuery,
            IUpdateProductCommand updateCommand,
            IDeleteProductCommand deleteCommand)
        {
            _createCommand = createCommand;
            _getProductsQuery = getProductsQuery;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _getProductsQuery.GetAllProductsAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _getProductsQuery.GetProductByIdAsync(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _createCommand.ExecuteAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error creating product: {ex.Message}");
                }
            }
            return View(dto);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _getProductsQuery.GetProductByIdAsync(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _updateCommand.ExecuteAsync(id, dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating product: {ex.Message}");
                }
            }
            return View(dto);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _getProductsQuery.GetProductByIdAsync(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _deleteCommand.ExecuteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

