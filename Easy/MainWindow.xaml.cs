using System;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Presentation;
using Easy.Constants;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using Microsoft.Practices.Unity;
namespace Easy
{
    public partial class MainWindow : ModernWindow
    {
        public const string Key = "MainWindow";
        public MainWindow(IUnityContainer container)
        {
            var links = container.ResolveAll<INavigateProvider>();

            InitializeComponent();
            Application.Current.Resources[Key] = this;
            foreach (var link in links.Where(x => x.Name == NavigateProvider.Main))
            {

                AddLinkGroups(new LinkGroupCollection() { link.GetLinkGroup() });
            }
            foreach (var link in links.Where(x => x.Name == NavigateProvider.Title))
            {
                AddTitleLinks(link.GetLinkGroup().Links);
            }
            AppearanceManager.Current.ThemeSource = new Uri(ThemesPath.DM, UriKind.Relative);

            ExitCommand = new Commands.Command(() =>{
                App.Current.Shutdown(); ;
        });

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            QQRob.Services.ClientManager.CLoseAll();
            Application.Current.Resources.Remove(Key);
            Application.Current.Shutdown();

        }
        public System.Windows.Input.ICommand ExitCommand { get; set; }
        public void AddLinkGroups(LinkGroupCollection linkGroupCollection)
        {
           // CreateMenuLinkGroup();

            foreach (LinkGroup linkGroup in linkGroupCollection)
            {
                this.MenuLinkGroups.Add(linkGroup);
            }
        }
        public void AddTitleLinks(LinkCollection linkCollection)
        {
            // CreateMenuLinkGroup();

            foreach (var  link in linkCollection)
            {
                this.TitleLinks.Add(link);
            }
        }/*
        private void CreateMenuLinkGroup()
        {
           this.MenuLinkGroups.Clear();

            LinkGroup linkGroup;= new LinkGroup
            {
                DisplayName = "Settings",
                GroupKey = "settings"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Settings options",
                Source = GetUri(typeof(Settings))
            });

            this.MenuLinkGroups.Add(linkGroup);

            linkGroup = new LinkGroup
            {
                DisplayName = "Common"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Dynamic Modules",
                Source = GetUri(typeof(IntroductionView))
            });

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Modern UI for WPF",
                Source = GetUri(typeof(MUIView))
            });

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Lorem Ipsum",
                Source = GetUri(typeof(LoremIpsumView))
            });

            this.MenuLinkGroups.Add(linkGroup);
        }*/

        private Uri GetUri(Type viewType)
        {
            return new Uri($"/Views/{viewType.Name}.xaml", UriKind.Relative);
        }
    }
}