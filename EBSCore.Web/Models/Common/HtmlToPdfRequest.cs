namespace EBSCore.Web.Models
{
    public class HtmlToPdfRequest
    {
        public string? Html { get; set; }
        public string? Title { get; set; }
        public string? FileName { get; set; }
    }
}
