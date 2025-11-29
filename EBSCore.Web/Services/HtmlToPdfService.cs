using HTMLQuestPDF.Extensions;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EBSCore.Web.Services
{
    public class HtmlToPdfService
    {
        public byte[] ConvertHtmlToPdf(string htmlContent, string? title = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var safeHtml = htmlContent ?? string.Empty;
            var documentTitle = string.IsNullOrWhiteSpace(title) ? "HTML Report" : title.Trim();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(11));

                    page.Header()
                        .Text(documentTitle)
                        .SemiBold()
                        .FontSize(16)
                        .FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(10)
                        .Column(col =>
                        {
                            col.Item().HTML(handler =>
                            {
                                // This is the correct call for QuestPDF.HTML 1.4.2
                                handler.SetHtml(safeHtml);
                            });
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
