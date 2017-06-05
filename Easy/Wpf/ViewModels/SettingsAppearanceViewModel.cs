using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;
using FirstFloor.ModernUI.Presentation;
using Easy.Constants;
using Easy.Models;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
namespace Easy.ViewModels
{
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class SettingsAppearanceViewModel : ViewModelBase
    {
        private SettingModel Setting;
        // 20 accent colors from Windows Phone 8
        private IEnumerable<ThemeColor> colors;
        private ThemeColor selectedAccentColor;
        private LinkCollection themes = new LinkCollection();
        private Link selectedTheme;
        private string selectedFontSize;

        public SettingsAppearanceViewModel()//:base(container)IUnityContainer container
        {
            Setting = OrmManager.GetDefault<SettingModel>();
            if (Setting == null)
            {
                OrmManager.Insert(new SettingModel());
                Setting= OrmManager.GetDefault<SettingModel>();
            }
             foreach (var theme in Theme.GetThemes())
            {
                this.themes.Add(new Link { DisplayName=theme.Name,Source=new Uri(theme.Path,UriKind.Relative) });
            }
            colors = Theme.GetThemeColors();
            FontSizes = Theme.GetThemeFrontSizes().Select(x=>x.Name);
            this.SelectedFontSize = Setting.FontSize; 
            this.SelectedTheme = this.themes.FirstOrDefault(l => l.DisplayName.Equals(Setting.Theme,StringComparison.InvariantCultureIgnoreCase));
            this.SelectedAccentColor = this.colors.FirstOrDefault(x=>x.Name.Equals(Setting.Color,StringComparison.InvariantCultureIgnoreCase));
                SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

        private void SyncThemeAndColor()
        {
  
            Setting.Color = this.SelectedAccentColor.Name;
            Setting.Theme = this.SelectedTheme.DisplayName;
            Setting.FontSize =Theme.GetThemeFrontSizeByFont( AppearanceManager.Current.FontSize).Name;
            OrmManager.Update(Setting);
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor"||e.PropertyName== "FontSize")
            {
                SyncThemeAndColor();
            }
        }

        public LinkCollection Themes
        {
            get { return this.themes; }
        }

        public IEnumerable<ThemeColor> AccentColors
        {
            get { return this.colors; }
        }
        public IEnumerable< string> FontSizes { get; private set; }
        public Link SelectedTheme
        {
            get { return this.selectedTheme; }
            set
            {
                if (this.selectedTheme != value)
                {
                    SetProperty(ref selectedTheme, value);

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;

                }
            }
        }

        public string SelectedFontSize
        {
            get { return this.selectedFontSize; }
            set
            {
                if (this.selectedFontSize != value)
                {
                    SetProperty(ref selectedFontSize, value);

                    var fontsize = Theme.GetThemeFrontSize(selectedFontSize);
                    AppearanceManager.Current.FontSize = fontsize.FontSize;
                    Application.Current.Resources[AppearanceManager.KeyDefaultFontSize] = fontsize.DefaultFontSize; 
                    Application.Current.Resources[AppearanceManager.KeyFixedFontSize] = fontsize.FixedFontSize;
                }
            }
        }

        public ThemeColor SelectedAccentColor
        {
            get { return this.selectedAccentColor; }
            set
            {
                if (this.selectedAccentColor != value)
                {
                    SetProperty(ref selectedAccentColor, value);
                   AppearanceManager.Current.AccentColor = value.ColorAlias;
                }
            }
        }
    }
}
