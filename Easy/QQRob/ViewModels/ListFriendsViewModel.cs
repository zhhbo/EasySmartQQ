using System.Collections.Generic;
using Easy.QQRob.Services;
using System.Windows.Input;
using Easy.Commands;
using Easy.QQRob.Models;
namespace Easy.QQRob.ViewModels
{
    public class ListFriendsViewModel : ViewModelBase
    {
        public ListFriendsViewModel() //ISmartService smartService: base(container)IUnityContainer container
        {
            Friends =  OrmManager.Fetch<FriendInfo>(x=> x.QQNum == WorkContext.GetState<long>(Constract.CurrentQQ));
            SelectedCommand = new Command(() =>{
                Logger.Log(SelectedFriend.Nick,Prism.Logging.Category.Debug, Prism.Logging.Priority.High);
            }, ()=>SelectedFriend!=null);
            RefreshCommand = new Command(() => {
                var state = WorkContext.GetState<QQState>(Constract.CurrentQQState);
                state?.GetClient().InitFriendList();
                //smartService.InitFriendList();
                Friends = OrmManager.Fetch<FriendInfo>(x => x.QQNum == WorkContext.GetState<long>(Constract.CurrentQQ));
                RaisePropertyChanged(nameof(Friends));
            });
            RefreshSelectedCommand = new Command(() => {
                var state = WorkContext.GetState<QQState>(Constract.CurrentQQState);
                state?.GetClient().InitGroupInfo(SelectedFriend.Uin);
               // smartService.InitFriendInfo(SelectedFriend.Uin);
                Friends = OrmManager.Fetch<FriendInfo>(x => x.QQNum == WorkContext.GetState<long>(Constract.CurrentQQ));
                RaisePropertyChanged(nameof(Friends));
            }, () => SelectedFriend != null);
        }
        public  IEnumerable<FriendInfo> Friends { get; set; }
        public FriendInfo SelectedFriend { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand RefreshSelectedCommand { get; set; }
    }
}
