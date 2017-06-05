using System.Collections.Generic;
using System.Linq;
using Easy.QQRob.Services;
using Easy.QQRob.Models;
using System.Windows.Input;
using Easy.Commands;
namespace Easy.QQRob.ViewModels
{
    public class DiscussionMessageSettingViewModel:ViewModelBase
    {
        public DiscussionMessageSettingViewModel(IEnumerable<IAutoAnswer> answers)
        {
            Answers = answers;
            Settings = answers.Select(x => new SettingInfo { Name = x.Name, Key = x.SettingKey }).ToList();
            Items = OrmManager.Fetch<DiscussInfo>(x => x.StillIn && x.QQNum == WorkContext.GetState<long>(Constract.CurrentQQ));
            SavedCommand = new Command(() =>
            {
                foreach (var setting in Settings)
                {
                    SelectedItem.SetState(setting.Key, setting.Selected);
                }
                OrmManager.Update(SelectedItem);

            });
            SelectedCommand = new Command(() =>
            {
                if (string.IsNullOrEmpty(SelectedItem.State))
                {
                    SelectedItem.State = WorkContext.GetState<QQState>(Constract.CurrentQQState).State;
                }
                for (int i = 0; i < Settings.Count; i++)
                {
                    Settings[i].Selected = SelectedItem.GetState<bool>(Settings[i].Key);
                }

                RaisePropertyChanged(nameof(Settings));
                PrevSelectedItem = SelectedItem;
            }, () => SelectedItem != PrevSelectedItem && SelectedItem != null);
        }

        public IEnumerable<DiscussInfo> Items { get; set; }
        public DiscussInfo PrevSelectedItem { get; set; }
        public DiscussInfo SelectedItem { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand SavedCommand { get; set; }
        public IEnumerable<IAutoAnswer> Answers { get; set; }
        public IList<SettingInfo> Settings { get; set; }
        public long CurrentQQ { get; set; }
        protected override void OnLoaded()
        {
            CurrentQQ = WorkContext.GetState<long>(Constract.CurrentQQ);
            Items = OrmManager.Fetch<DiscussInfo>(x => x.StillIn&& x.QQNum == CurrentQQ);
            RaisePropertyChanged(nameof(Items));

        }
    }
}