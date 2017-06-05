using FirstFloor.ModernUI.Presentation;

namespace Easy
{
    public interface INavigateProvider:IEventHandler
    {
        LinkGroup GetLinkGroup();
        string Name { get; }
    }
    public static class NavigateProvider
    {
        public const string Setting = "Setting";
        public const string Main = "Main";
        public const string Title = "Title";
    }
}