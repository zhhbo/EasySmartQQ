using System;
using FirstFloor.ModernUI.Presentation;
using Easy.Views;

namespace Easy.Providers
{
    /// <summary>
    /// Creates a LinkGroup
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This is the entry point for the option menu.
    /// </remarks>
    public class SettingNavigateProvider : INavigateProvider
    {
        public LinkGroup GetLinkGroup()
        {
            LinkGroup linkGroup = new LinkGroup
            {
                DisplayName = "外观"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "外观",
                Source = new Uri($"/Views/{nameof(SettingsAppearance)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
        public string Name { get { return Easy.NavigateProvider.Setting; } }
    }
}
