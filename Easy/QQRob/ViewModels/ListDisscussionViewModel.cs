using Prism.Commands;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using Easy;
using Microsoft.Practices.Unity;
using Easy.QQRob.Services;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Easy.Commands;
using Prism.Regions;
using Easy.QQRob.Models;
using Prism.Logging;
using System.Threading.Tasks;
namespace Easy.QQRob.ViewModels
{
    public class ListDisscussionViewModel : ViewModelBase
    {
        public ListDisscussionViewModel(ILoggerFacade logger) //,ISmartService smartService: base(container)IUnityContainer container
        {
            Logger = logger;
            Items =  OrmManager.Fetch<DiscussInfo>(x=>x.StillIn);
            SelectedCommand = new Command(() =>{
                Logger.Debug(SelectedItem.Name);
            }, ()=>SelectedItem!=null);
            RefreshBasicCommand=new Command(() => {
               var state= WorkContext.GetState<QQState>(Constract.CurrentQQState);               
                    state?.GetClient().InitDisscussList();
                Items = OrmManager.Fetch<DiscussInfo>(x => x.QQNum==WorkContext.GetState<long>(Constract.CurrentQQ)&&x.StillIn);
                RaisePropertyChanged(nameof(Items));
            });
            RefreshDetailCommand = new Command(() => {

              
                Task.Run(() => {
                foreach (var group in Items)
                {
                        var state = WorkContext.GetState<QQState>(Constract.CurrentQQState);
                        state?.GetClient().InitDisscussInfo(group.Did);
                        // smartService.InitDisscussInfo(group.Did);
                    }
                });
                Items = OrmManager.Fetch<DiscussInfo>(x => x.QQNum == WorkContext.GetState<long>(Constract.CurrentQQ) && x.StillIn);
                RaisePropertyChanged(nameof(Items));

            },()=>Items.Count()>0);
            RefreshSelectedCommand = new Command(() => {
                // Logger.Debug(SelectedItem.Name);
                var state = WorkContext.GetState<QQState>(Constract.CurrentQQState);
                state?.GetClient().InitDisscussInfo(SelectedItem.Did);
                //smartService.InitDisscussInfo(SelectedItem.Did);
                Items = OrmManager.Fetch<DiscussInfo>(x => x.QQNum == WorkContext.GetState<long>(Constract.CurrentQQ) && x.StillIn);
                RaisePropertyChanged(nameof(Items));
              
            }, () => SelectedItem != null);
        }
        public  IEnumerable<DiscussInfo> Items { get; set; }
        public DiscussInfo SelectedItem { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand RefreshBasicCommand { get; set; }
        public ICommand RefreshSelectedCommand { get; set; }
        public ICommand RefreshDetailCommand { get; set; }
    }
}
