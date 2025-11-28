using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EBSCore.Web.Services
{
    public class HtmlToPdfService
    {
        private static bool _licenseApplied;

        public HtmlToPdfService()
        {
            if (!_licenseApplied)
            {
                Settings.License = LicenseType.Community;
                _licenseApplied = true;
            }
        }

        public byte[] ConvertHtmlToPdf(string htmlContent, string? title = null)
        {
            var safeHtml = htmlContent ?? string.Empty;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(TextStyle.Default.Size(12));

                    page.Header()
                        .Text(title ?? "HTML Report")
                        .SemiBold()
                        .FontSize(18)
                        .FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Background("#f9fafb").Padding(10).Border(1).BorderColor("#e5e7eb").RoundCorners(4);
                            column.Item().Html(safeHtml);
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
