using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

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

        private void MenuUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            lol.Focus(FocusState.Programmatic);
        }
    }
}
