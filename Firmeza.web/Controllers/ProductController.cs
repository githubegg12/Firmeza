using Firmeza.Application.Features.Product.Commands;
using Firmeza.Application.Features.Product.Queries;
using Firmeza.Application.Features.Product.Interfaces;
using Firmeza.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

using Firmeza.Application.Features.Pdf.Interfaces;
using OfficeOpenXml;

using Microsoft.AspNetCore.Authorization;

namespace Firmeza.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProductController : Controller
    {
        private readonly ICreateProductCommand _createCommand;
        private readonly IGetProductQuery _getProductQuery;
        private readonly IUpdateProductCommand _updateCommand;
        private readonly IDeleteProductCommand _deleteCommand;
        private readonly IPdfService _pdfService;

        public ProductController(
            ICreateProductCommand createCommand,
            IGetProductQuery getProductsQuery,
            IUpdateProductCommand updateCommand,
            IDeleteProductCommand deleteCommand,
            IPdfService pdfService)
        {
            _createCommand = createCommand;
            _getProductQuery = getProductsQuery;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
            _pdfService = pdfService;
        }

        // GET: Product
        public async Task<IActionResult> Index(string searchString, string categoryFilter)
        {
            var products = await _getProductQuery.GetAllProductsAsync();
            
            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => 
                    p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            
            // Apply category filter
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                products = products.Where(p => 
                    p.Category.Equals(categoryFilter, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            
            // Get unique categories for filter dropdown
            ViewBag.Categories = (await _getProductQuery.GetAllProductsAsync())
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
                
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = categoryFilter;
            
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _getProductQuery.GetProductByIdAsync(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
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

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _getProductQuery.GetProductByIdAsync(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Product/Edit/5
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
            
            // If we got here, something failed, reload the product
            var product = await _getProductQuery.GetProductByIdAsync(id);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _getProductQuery.GetProductByIdAsync(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Product/Delete/5
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


        // GET: Product/ExportToExcel
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var Product = await _getProductQuery.GetAllProductsAsync();
                
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Productos");

                // Headers
                // Headers (Must match BulkImportService expected keys)
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "ProductName";
                worksheet.Cells[1, 3].Value = "ProductCategory";
                worksheet.Cells[1, 4].Value = "UnitPrice";
                worksheet.Cells[1, 5].Value = "Stock";
                worksheet.Cells[1, 6].Value = "Description";
                worksheet.Cells[1, 7].Value = "ImagenUrl";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data
                int row = 2;
                foreach (var product in Product)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.Name;
                    worksheet.Cells[row, 3].Value = product.Category;
                    worksheet.Cells[row, 4].Value = product.Price;
                    worksheet.Cells[row, 5].Value = product.Stock;
                    worksheet.Cells[row, 6].Value = product.Description;
                    worksheet.Cells[row, 7].Value = product.ImageUrl;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var content = package.GetAsByteArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Productos_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting Excel: {ex}");
                return BadRequest($"Error exporting Excel: {ex.Message}");
            }
        }

        // GET: Product/ExportToPdf
        public async Task<IActionResult> ExportToPdf()
        {
            try
            {
                var Product = await _getProductQuery.GetAllProductsAsync();
                var pdfBytes = await _pdfService.GenerateProductListPdfAsync(Product);
                return File(pdfBytes, "application/pdf", $"Productos_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting PDF: {ex}");
                return BadRequest($"Error exporting PDF: {ex.Message}");
            }
        }
    }
}

