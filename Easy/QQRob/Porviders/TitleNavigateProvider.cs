using System;
using FirstFloor.ModernUI.Presentation;
using Easy;
using Easy.QQRob.Views;
namespace Easy.QQRob.Providers
{
    /// <summary>
    /// Creates a LinkGroup
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This is the entry point for the option menu.
    /// </remarks>
    public class TitleNavigateProvider : INavigateProvider
    {
        public LinkGroup GetLinkGroup()
        {
            LinkGroup linkGroup = new LinkGroup
            {
                DisplayName = "模块"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "模块",
                Source = new Uri($"/QQRob/Views/{nameof(ListFriends)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
        public string Name { get { return Easy.NavigateProvider.Title; } }
    }
}
