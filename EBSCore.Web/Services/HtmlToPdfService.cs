using HtmlRenderer.PdfSharp;
using PdfSharpCore;
using PdfSharpCore.Pdf;

namespace EBSCore.Web.Services
{
    public class HtmlToPdfService
    {
        public byte[] ConvertHtmlToPdf(string htmlContent, string? title = null)
        {
            var safeHtml = htmlContent ?? string.Empty;
            var pdfConfig = new PdfGenerateConfig
            {
                PageSize = PageSize.A4,
                MarginTop = 20,
                MarginBottom = 20,
                MarginLeft = 20,
                MarginRight = 20,
                Title = title ?? "HTML Report"
            };

            var pdfDocument = PdfGenerator.GeneratePdf(safeHtml, pdfConfig);

            using var stream = new MemoryStream();
            pdfDocument.Save(stream, false);
            return stream.ToArray();
        }
    }
}
