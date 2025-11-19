using Firmeza.Application.Features.BulkImport;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.web.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly IBulkImportService _bulkImportService;

        public BulkImportController(IBulkImportService bulkImportService)
        {
            _bulkImportService = bulkImportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.Error = "Please select a file to upload.";
                return View("Index");
            }

            using var stream = excelFile.OpenReadStream();
            var result = await _bulkImportService.ProcessExcelFileAsync(stream);

            // The result is now a BulkImportResultDto, which the view can use.
            return View("Index", result);
        }
    }
}



