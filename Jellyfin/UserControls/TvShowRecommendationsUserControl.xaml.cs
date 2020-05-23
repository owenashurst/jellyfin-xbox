using Windows.UI.Xaml;

namespace Jellyfin.UserControls
{
    public sealed partial class TvShowRecommendationsUserControl
    {
        public TvShowRecommendationsUserControl()
        {
            InitializeComponent();
        }

        private void UIElement_OnGotFocus1(object sender, RoutedEventArgs e)
        {
           //b.LoseFocus();
           //c.LoseFocus();
           //d.LoseFocus();
        }
        private void UIElement_OnGotFocus2(object sender, RoutedEventArgs e)
        {
            a.LoseFocus();
            //c.LoseFocus();
            //d.LoseFocus();
        }
        private void UIElement_OnGotFocus3(object sender, RoutedEventArgs e)
        {
            //b.LoseFocus();
            a.LoseFocus();
            //d.LoseFocus();
        }

        private void UIElement_OnGotFocus4(object sender, RoutedEventArgs e)
        {
            //b.LoseFocus();
            a.LoseFocus();
            //c.LoseFocus();
        }
    }
}
