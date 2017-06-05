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
    public class ModuleNavigateProvider : INavigateProvider
    {
        public LinkGroup GetLinkGroup()
        {
            LinkGroup linkGroup = new LinkGroup
            {
                DisplayName = "QQ机器人"
            };
            linkGroup.Links.Add(new Link
            {
                DisplayName = "登录过的帐户",
                Source = new Uri($"/QQRob/Views/{nameof(ListQQState)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "登录",
                Source = new Uri($"/QQRob/Views/{nameof(Login)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "群",
                Source = new Uri($"/QQRob/Views/{nameof(ListGroup)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "讨论组",
                Source = new Uri($"/QQRob/Views/{nameof(ListDisscussion)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "好友",
                Source = new Uri($"/QQRob/Views/{nameof(ListFriends)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "系统日志",
                Source = new Uri($"/QQRob/Views/{nameof(ListSystemMessageLog)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "收到消息日志",
                Source = new Uri($"/QQRob/Views/{nameof(ListReciveMessage)}.xaml", UriKind.Relative)
            });
            linkGroup.Links.Add(new Link
            {
                DisplayName = "自动回复日志",
                Source = new Uri($"/QQRob/Views/{nameof(ListAutoAnswerMessage)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
        public string Name { get { return "Easy.QQRob"; } }
    }
}
