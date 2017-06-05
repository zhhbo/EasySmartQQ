using System.Collections.Generic;
using System.Linq;
using Easy.QQRob.Models;
using System.Windows.Input;
using Easy.QQRob.Services;
using Easy.Commands;
namespace Easy.QQRob.ViewModels
{
    public  class GroupMessageSettingViewModel:ViewModelBase
    {
        public GroupMessageSettingViewModel(IEnumerable<IAutoAnswer> answers)
        {
            Answers = answers;
            Settings = answers.Select(x=>new SettingInfo { Name=x.Name, Key=x.SettingKey }).ToList();
            Items = OrmManager.Fetch<GroupInfo>(x => x.StillIn&&x.QQNum==WorkContext.GetState<long>(Constract.CurrentQQ));
            SavedCommand = new Command(() =>{
                foreach (var setting in Settings)
                {
                    SelectedItem.SetState(setting.Key, setting.Selected);
                }
                OrmManager.Update(SelectedItem);

            });
            SelectedCommand = new Command(() => {
                if (string.IsNullOrEmpty( SelectedItem.State))
                {
                    SelectedItem.State = WorkContext.GetState<QQState>(Constract.CurrentQQState).State;
                }
                    for (int i = 0; i < Settings.Count; i++)
                    {
                        Settings[i].Selected = SelectedItem.GetState<bool>(Settings[i].Key);
                }
                
                RaisePropertyChanged(nameof(Settings));
                PrevSelectedItem = SelectedItem;
            },()=>SelectedItem!=PrevSelectedItem&&SelectedItem!=null);
        }

        public IEnumerable<GroupInfo> Items { get; set; }
        public GroupInfo PrevSelectedItem { get; set; }
        public GroupInfo SelectedItem { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand SavedCommand { get; set; }
        public IEnumerable<IAutoAnswer> Answers { get; set; }
        public IList<SettingInfo> Settings { get; set; }
        public long CurrentQQ { get; set; }
        protected override void OnLoaded()
        {
            CurrentQQ = WorkContext.GetState<long>(Constract.CurrentQQ);
            Items = OrmManager.Fetch<GroupInfo>(x => x.StillIn&&x.QQNum==CurrentQQ);
            RaisePropertyChanged(nameof(Items));

        }


    }
    public class SettingInfo:ViewModelBase
    {
        private bool isSelected = false;
        public bool Selected {
            get { return isSelected; }
            set { isSelected = value;
                    RaisePropertyChanged("Selected");
            }
        } 
        
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
