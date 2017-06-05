using Prism.Commands;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using Easy;
using Microsoft.Practices.Unity;
namespace Easy.QQRob.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IEnumerable<INavigateProvider> navigates)// : base(container)IUnityContainer container, 
        {
            Links = new LinkCollection();

            var links = navigates.Where(x => x.Name == "Easy.QQRob");
            foreach (var link in links.SelectMany(x => x.GetLinkGroup().Links))
            {
                Links.Add(link);
            }
            SelectedLink = Links.FirstOrDefault()?.Source;
        }
        public LinkCollection Links { get; set; }
        public Uri SelectedLink { get; set; }
    }
}
