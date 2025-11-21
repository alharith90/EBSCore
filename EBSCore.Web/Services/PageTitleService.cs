using System;
using System.Threading.Tasks;

namespace EBSCore.Web.Services
{
    public class PageTitleService
    {
        private string _currentPageTitle = string.Empty;
        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set
            {
                _currentPageTitle = value;
                PageTitleChanged?.Invoke(value);
            }
        }

        public event Action<string>? PageTitleChanged;

        public Task SetPageTitle(string title)
        {
            CurrentPageTitle = title;
            return Task.CompletedTask;
        }

        public void SetTitle(string title)
        {
            CurrentPageTitle = title;
        }
    }
}
