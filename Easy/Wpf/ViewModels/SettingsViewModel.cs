using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using FirstFloor.ModernUI.Presentation;
using System;
using Microsoft.Practices.Unity;
namespace Easy.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(IEnumerable<INavigateProvider> navigates)//:base(container),IUnityContainer container
        {
            Links = new LinkCollection();

            var links = navigates.Where(x => x.Name == NavigateProvider.Setting);
            foreach (var link in links.SelectMany(x=>x.GetLinkGroup().Links))
            {
                Links.Add(link);
            }
            SelectedLink = Links.FirstOrDefault()?.Source;
            
        }
        public LinkCollection Links { get; set; }
        public Uri SelectedLink { get; set; }
    }
}
