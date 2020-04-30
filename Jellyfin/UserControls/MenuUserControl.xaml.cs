using Windows.UI.Xaml;

namespace Jellyfin.UserControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuUserControl
    {
        public MenuUserControl()
        {
            InitializeComponent();
        }

        private void MenuUserControl_OnGotFocus(object sender, RoutedEventArgs e)
        {
            sender.ToString();
        }

        private void MenuUserControl_OnLostFocus(object sender, RoutedEventArgs e)
        {
            sender.ToString();
        }
    }
}
