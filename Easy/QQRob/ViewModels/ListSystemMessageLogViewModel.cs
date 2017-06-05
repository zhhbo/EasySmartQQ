using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using Easy.Commands;
using Easy.QQRob.Models;
using Prism.Logging;
namespace Easy.QQRob.ViewModels
{
    public class ListSystemMessageLogViewModel : ViewModelBase
    {
        public ListSystemMessageLogViewModel(ILoggerFacade logger ) //: base(container)IUnityContainer container
        {
            Logger = logger;
            Logs =  OrmManager.Fetch<SystemLogData>(x=>x.Id>0).Take(PageSize);
            TotalPage =( OrmManager.Count<SystemLogData>()+ PageSize-1)/PageSize;
            RaisePropertyChanged(nameof(CurrentPage));
            RaisePropertyChanged(nameof(Logs));
            SelectedCommand = new Command<TextBlock>(detailData =>{
                detailData.Text = SelectedLog.OriginData;
                Logger.Log(SelectedLog.OriginData, Category.Debug, Priority.High);
            }, detailData => SelectedLog != null);
            GoToPageCommand = new Command(() => {

                Logs = OrmManager.Fetch<SystemLogData>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage >= 1);
            FirstPageCommand = new Command(() => {
                CurrentPage = 1;
                Logs = OrmManager.Fetch<SystemLogData>(x => x.Id > 0).Skip(PageSize* (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage!=1&& CurrentPage <= TotalPage);
            NextPageCommand = new Command(() => {
                CurrentPage ++;
                if (CurrentPage > TotalPage)
                    CurrentPage = TotalPage;
                 Logs = OrmManager.Fetch<SystemLogData>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage <TotalPage);
            PrevPageCommand = new Command(() => {
                CurrentPage--;
                Logs = OrmManager.Fetch<SystemLogData>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage >1&& CurrentPage <= TotalPage);
            LastPageCommand = new Command(() => {
                CurrentPage = TotalPage;
                Logs = OrmManager.Fetch<SystemLogData>(x => x.Id > 0).Skip(PageSize *( CurrentPage-1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage < TotalPage);
        }
        public  IEnumerable<SystemLogData> Logs { get; set; }
        public SystemLogData SelectedLog { get; set; }
        public ICommand SelectedCommand { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPage { get; set; }
        public ICommand GoToPageCommand { get; set; }
        public ICommand FirstPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PrevPageCommand { get; set; }
        public ICommand LastPageCommand { get; set; }
        public int PageSize { get; set; } = 10;
        protected override void OnLoaded()
        {
            TotalPage = (OrmManager.Count<SystemLogData>() + PageSize - 1) / PageSize;
            // base.OnLoaded();
            Logs = OrmManager.Fetch<SystemLogData>(x => x.Id > 0).Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            RaisePropertyChanged(nameof(TotalPage));
            RaisePropertyChanged(nameof(Logs));
        }
    }
}
