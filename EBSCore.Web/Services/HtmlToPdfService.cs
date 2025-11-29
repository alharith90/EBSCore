using EBSCore.Web.Models;
using HTMLQuestPDF.Extensions;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EBSCore.Web.Services
{
    public class HtmlToPdfService
    {
        public byte[] ConvertHtmlToPdf(HtmlToPdfRequest? request)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var htmlContent = request?.Html ?? string.Empty;
            var documentTitle = "HTML Report";

            if (!string.IsNullOrWhiteSpace(request?.Title))
            {
                documentTitle = request.Title!.Trim();
            }

            var margin = request?.Margin is > 0 ? request.Margin : 20;
            var includePageNumbers = request?.ShowPageNumbers ?? true;

            var pageSize = request?.PageSize?.ToUpperInvariant() switch
            {
                "LETTER" => PageSizes.Letter,
                "LEGAL" => PageSizes.Legal,
                _ => PageSizes.A4
            };

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(pageSize);
                    page.Margin(margin);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(11));

                    page.Header().Element(header =>
                    {
                        if (!string.IsNullOrWhiteSpace(request?.HeaderHtml))
                        {
                            header.HTML(html => html.SetHtml(request.HeaderHtml));
                        }
                        else
                        {
                            header
                                .Text(documentTitle)
                                .SemiBold()
                                .FontSize(16)
                                .FontColor(Colors.Blue.Medium);
                        }
                    });

                    page.Content()
                        .PaddingVertical(10)
                        .Column(col =>
                        {
                            col.Item().HTML(handler => handler.SetHtml(htmlContent));
                        });

                    page.Footer().Element(footer =>
                    {
                        if (!string.IsNullOrWhiteSpace(request?.FooterHtml))
                        {
                            footer.HTML(html => html.SetHtml(request.FooterHtml));
                            return;
                        }

                        if (!includePageNumbers)
                        {
                            return;
                        }

                        footer.AlignCenter().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
