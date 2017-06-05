using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
namespace Easy.Constants
{
    public struct ThemesPath
    {
        public const string Light = "/Wpf/Themes/LightTheme.xaml";
        public const string Dark = "/Wpf/Themes/DarkTheme.xaml";
        public const string DM = "/Wpf/Themes/DMTheme.xaml";
        public const string LightBingImage = "/Wpf/Themes/LightBingImageTheme.xaml";
        public const string DarkBingImage = "/Wpf/Themes/DarkBingImageTheme.xaml";
        public const string Windows10 = "/Wpf/Themes/Windows10Theme.xaml";
        public const string MacOSX = "/Wpf/Themes/MacOSXTheme.xaml";
    }
    public class Theme
    {
        public static List<Theme> GetThemes()
        {
            var list= new List<Theme>();
            list.Add(new Theme("Light",ThemesPath.Light));
            list.Add(new Theme("Dark", ThemesPath.Dark));
            list.Add(new Theme("Easy", ThemesPath.DM));
            list.Add(new Theme("LightBing", ThemesPath.LightBingImage));
            list.Add(new Theme("DarkBing", ThemesPath.DarkBingImage));
            list.Add(new Theme("Win10", ThemesPath.Windows10));
            list.Add(new Theme("Mac", ThemesPath.MacOSX));

            return list;
        }
        public static Theme GetTheme(string name)
        {
            return GetThemes().FirstOrDefault(x=>string.Equals(x.Name,name,StringComparison.InvariantCultureIgnoreCase));
        }
        public static Theme GetThemeByPath(string path)
        {
            return GetThemes().FirstOrDefault(x => string.Equals(x.Path, path, StringComparison.InvariantCultureIgnoreCase));
        }
        public static List<ThemeColor> GetThemeColors()
        {
            var list = new List<ThemeColor>();
            list.Add(new ThemeColor(Color.FromRgb(0xa4, 0xc4, 0x00), "lime"));
            list.Add(new ThemeColor(Color.FromRgb(0x87, 0x79, 0x4e), "taupe"));
            list.Add(new ThemeColor(Color.FromRgb(0x76, 0x60, 0x8a), "mauve"));
            list.Add(new ThemeColor(Color.FromRgb(0x64, 0x76, 0x87), "steel"));
            list.Add(new ThemeColor(Color.FromRgb(0x6d, 0x87, 0x64), "olive"));
            list.Add(new ThemeColor(Color.FromRgb(0x82, 0x5a, 0x2c), "brown"));
            list.Add(new ThemeColor(Color.FromRgb(0xe3, 0xc8, 0x00), "yellow"));
            list.Add(new ThemeColor(Color.FromRgb(0xf0, 0xa3, 0x0a), "amber"));
            list.Add(new ThemeColor(Color.FromRgb(0xfa, 0x68, 0x00), "orange"));
            list.Add(new ThemeColor(Color.FromRgb(0xe5, 0x14, 0x00), "red"));
            list.Add(new ThemeColor(Color.FromRgb(0xa2, 0x00, 0x25), "crimson"));
            list.Add(new ThemeColor(Color.FromRgb(0xd8, 0x00, 0x73), "magenta"));
            list.Add(new ThemeColor(Color.FromRgb(0x60, 0xa9, 0x17), "green"));
            list.Add(new ThemeColor(Color.FromRgb(0x00, 0x8a, 0x00), "emerald"));
            list.Add(new ThemeColor(Color.FromRgb(0x00, 0xab, 0xa9), "teal"));
            list.Add(new ThemeColor(Color.FromRgb(0x1b, 0xa1, 0xe2), "cyan"));
            list.Add(new ThemeColor(Color.FromRgb(0x00, 0x50, 0xef), "cobalt"));
            list.Add(new ThemeColor(Color.FromRgb(0x6a, 0x00, 0xff), "indigo"));
            list.Add(new ThemeColor(Color.FromRgb(0xaa, 0x00, 0xff), "violet"));
            list.Add(new ThemeColor(Color.FromRgb(0xf4, 0x72, 0xd0), "pink"));
            return list;
        }
        public static ThemeColor GetThemeColorByName(string name)
        {
            return GetThemeColors().FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }
        public static ThemeColor GetThemeColorByColor(Color color)
        {
            return GetThemeColors().FirstOrDefault(x => Color.Equals(x.ColorAlias, color));
        }
        public static List<ThemeFrontSize> GetThemeFrontSizes()
        {
            var list = new List<ThemeFrontSize>();
            list.Add(new ThemeFrontSize( "small", 13D, 12.667D,FontSize.Small));
            list.Add(new ThemeFrontSize( "large", 16D, 16.333D,FontSize.Large));

            return list;
        }
        public static ThemeFrontSize GetThemeFrontSize(string name)
        {
            return GetThemeFrontSizes().FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }
        public static ThemeFrontSize GetThemeFrontSizeByFont(FontSize size)
        {
            return GetThemeFrontSizes().FirstOrDefault(x => x.FontSize==size);
        }
        public Theme(string name,string path)
        {
            Name = name;
            Path = path;
        }
        public string Path { get; private set; }
        public string Name { get; private set; }
    }
    public class ThemeColor
    {


        public ThemeColor(Color color,string name)
        {
            Name = name;
        ColorAlias = color;
        }
        public Color ColorAlias { get; private set; }
        public string Name { get; private set; }
    }
    public class ThemeFrontSize
    {


        public ThemeFrontSize( string name, double defaultFontSize, double fixedFontSize,FontSize size)
        {
            Name = name;
            DefaultFontSize = defaultFontSize;
            FixedFontSize = fixedFontSize;
            FontSize = size;
        }
        public double DefaultFontSize { get; private set; }
        public double FixedFontSize { get; private set; }
        public string Name { get; private set; }
        public FontSize FontSize { get; private set; }
    }
}
