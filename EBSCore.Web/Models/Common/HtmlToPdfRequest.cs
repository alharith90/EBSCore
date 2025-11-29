namespace EBSCore.Web.Models
{
    public class HtmlToPdfRequest
    {
        public string? Html { get; set; }
        public string? Title { get; set; }
        public string? FileName { get; set; }
        public string? HeaderHtml { get; set; }
        public string? FooterHtml { get; set; }
        public string? PageSize { get; set; }
        public float Margin { get; set; } = 20;
        public bool ShowPageNumbers { get; set; } = true;
    }
}
