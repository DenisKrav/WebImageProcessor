namespace WebImageProcessor.ViewModel.Models
{
    public class MenuItemModel
    {
        public string Title { get; }
        public string IconClass { get; }
        public string MenuTitle { get; }
        public string Url { get; }

        public MenuItemModel(string title, string iconClass, string menuTitle, string url)
        {
            Title = title;
            IconClass = iconClass;
            MenuTitle = menuTitle;
            Url = url;
        }
    }
}
