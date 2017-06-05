using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy.QQRob.Services;
using Easy.QQRob.Models;
using Easy.Commands;
using System.Windows.Input;
namespace Easy.QQRob.ViewModels
{
   public  class DefaultMessageSettingViewModel:ViewModelBase
    {
        public DefaultMessageSettingViewModel(IEnumerable<IAutoAnswer> answers)
        {
            Answers = answers;
            Settings = answers.Select(x => new SettingInfo { Name = x.Name, Key = x.SettingKey }).ToList();
            Items = OrmManager.Fetch<QQState>(x => x.Id > 0);
            SavedCommand = new Command(() => {
                foreach (var setting in Settings)
                {
                    SelectedItem.SetState(setting.Key, setting.Selected);
                }
                OrmManager.Update(SelectedItem);
                var client = SelectedItem.GetClient();
                if(client!=null)
                    client.DefaultState = SelectedItem.State;

            });
            SelectedCommand = new Command(() => {
                if (string.IsNullOrEmpty(SelectedItem.State))
                {
                    SelectedItem.State = "{}";
                }
                for (int i = 0; i < Settings.Count; i++)
                {
                    Settings[i].Selected = SelectedItem.GetState<bool>(Settings[i].Key);
                }

                RaisePropertyChanged(nameof(Settings));
                PrevSelectedItem = SelectedItem;
            }, () => SelectedItem != PrevSelectedItem && SelectedItem != null);
        }

        public IEnumerable<QQState> Items { get; set; }
        public QQState PrevSelectedItem { get; set; }
        public QQState SelectedItem { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand SavedCommand { get; set; }
        public IEnumerable<IAutoAnswer> Answers { get; set; }
        public IList<SettingInfo> Settings { get; set; }
        protected override void OnLoaded()
        {

            Items = OrmManager.Fetch<QQState>(x => x.Id > 0);
            RaisePropertyChanged(nameof(Items));

        }
    }
}
