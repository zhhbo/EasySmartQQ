using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using Easy.Commands;
using Easy.QQRob.Models;
using Prism.Logging;
namespace Easy.QQRob.ViewModels
{
    public class ListAutoAnswerMessageViewModel : ViewModelBase
    {
        public ListAutoAnswerMessageViewModel(ILoggerFacade logger) //: base(container)IUnityContainer container
        {
            Logger = logger;
            Logs = OrmManager.Fetch<AutoAnswerMessageLog>(x => x.Id > 0).Take(PageSize);
            TotalPage = (OrmManager.Count<AutoAnswerMessageLog>() + PageSize - 1) / PageSize;
            RaisePropertyChanged(nameof(CurrentPage));
            RaisePropertyChanged(nameof(Logs));
            SelectedCommand = new Command<TextBlock>(detailData => {
                detailData.Text = SelectedLog.Data;
                Logger.Log(SelectedLog.Data, Category.Debug, Priority.High);
            }, detailData => SelectedLog != null);
            FirstPageCommand = new Command(() => {
                CurrentPage = 1;
                Logs = OrmManager.Fetch<AutoAnswerMessageLog>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage != 1 && CurrentPage <= TotalPage);
            NextPageCommand = new Command(() => {
                CurrentPage++;
                if (CurrentPage > TotalPage)
                    CurrentPage = TotalPage;
                Logs = OrmManager.Fetch<AutoAnswerMessageLog>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage < TotalPage);
            PrevPageCommand = new Command(() => {
                CurrentPage--;
                Logs = OrmManager.Fetch<AutoAnswerMessageLog>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage > 1 && CurrentPage <= TotalPage);
            LastPageCommand = new Command(() => {
                CurrentPage = TotalPage;
                Logs = OrmManager.Fetch<AutoAnswerMessageLog>(x => x.Id > 0).Skip(PageSize * (CurrentPage - 1)).Take(PageSize);
                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(Logs));
                Logger.Log(CurrentPage.ToString(), Category.Debug, Priority.High);
            }, () => CurrentPage < TotalPage);
        }
        public IEnumerable<AutoAnswerMessageLog> Logs { get; set; }
        public AutoAnswerMessageLog SelectedLog { get; set; }
        public ICommand SelectedCommand { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPage { get; set; }
        public ICommand FirstPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PrevPageCommand { get; set; }
        public ICommand LastPageCommand { get; set; }
        public int PageSize { get; set; } = 10;
        protected override void OnLoaded()
        {
            TotalPage = (OrmManager.Count<AutoAnswerMessageLog>() + PageSize - 1) / PageSize;
            // base.OnLoaded();
            Logs = OrmManager.Fetch<AutoAnswerMessageLog>(x => x.Id > 0).Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            RaisePropertyChanged(nameof(TotalPage));
            RaisePropertyChanged(nameof(Logs));
        }
    }
}
