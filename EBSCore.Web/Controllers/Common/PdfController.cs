using EBSCore.Web.Models;
using EBSCore.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PdfController : ControllerBase
    {
        private readonly HtmlToPdfService _htmlToPdfService;
        private readonly ILogger<PdfController> _logger;

        public PdfController(HtmlToPdfService htmlToPdfService, ILogger<PdfController> logger)
        {
            _htmlToPdfService = htmlToPdfService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult FromHtml([FromBody] HtmlToPdfRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Html))
            {
                return BadRequest("HTML content is required");
            }

            try
            {
                var bytes = _htmlToPdfService.ConvertHtmlToPdf(request);
                var fileName = string.IsNullOrWhiteSpace(request.FileName) ? "report.pdf" : request.FileName;

                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".pdf";
                }

                return File(bytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate PDF from HTML");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to export PDF");
            }
        }
    }
}
