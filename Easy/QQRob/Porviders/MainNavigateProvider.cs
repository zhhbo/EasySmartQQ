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
    public class MainNavigateProvider : INavigateProvider
    {
        public LinkGroup GetLinkGroup()
        {
            LinkGroup linkGroup = new LinkGroup
            {
                DisplayName = "QQ机器人"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "QQ机器人",
                Source = new Uri($"/QQRob/Views/{nameof(Main)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "默认设置",
                Source = new Uri($"/QQRob/Views/{nameof(DefaultMessageSetting)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "讨论组设置",
                Source = new Uri($"/QQRob/Views/{nameof(DiscussionMessageSetting)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "群设置",
                Source = new Uri($"/QQRob/Views/{nameof(GroupMessageSetting)}.xaml", UriKind.Relative)
            });
            return linkGroup;
        }
        public string Name { get { return Easy.NavigateProvider.Main; } }
    }
}
