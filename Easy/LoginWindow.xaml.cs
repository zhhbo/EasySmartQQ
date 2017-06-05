using FirstFloor.ModernUI.Windows.Controls;
using System.ComponentModel;
using System.Windows;
namespace Easy
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : ModernWindow
    {
        public const string Key = "LoginWindow";
        public LoginWindow()
        {
            InitializeComponent();
            Application.Current.Resources[Key] = this;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Application.Current.Resources.Remove(Key);
        }
    }
}
