namespace EBSCore.Web.Services
{
    public class PageTitleService
    {
        private string _currentPageTitle;
        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set
            {
                _currentPageTitle = value;
                PageTitleChanged?.Invoke(value);
            }
        }
        public event Action<string> PageTitleChanged;
    }
}
