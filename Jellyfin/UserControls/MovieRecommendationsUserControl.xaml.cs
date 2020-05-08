using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Jellyfin.Core.Models;
using Jellyfin.Models;

namespace Jellyfin.UserControls
{
    public sealed partial class MovieRecommendationsUserControl
    {

        #region ItemsSource Dependency Property

        public static readonly DependencyProperty MediaSourceDependency =
            DependencyProperty.Register("MediaSource", typeof(ObservableCollection<ModelBase>),
                typeof(MovieRecommendationsUserControl), new PropertyMetadata(null));

        public ObservableCollection<ModelBase> MediaSource
        {
            get => (ObservableCollection<ModelBase>)GetValue(MediaSourceDependency);
            set => SetValue(MediaSourceDependency, value);
        }

        #endregion

        public MovieRecommendationsUserControl()
        {
            InitializeComponent();
        }

        private void UIElement_OnGotFocus1(object sender, RoutedEventArgs e)
        {
           b.LoseFocus();
           c.LoseFocus();
           d.LoseFocus();
        }
        private void UIElement_OnGotFocus2(object sender, RoutedEventArgs e)
        {
            a.LoseFocus();
            c.LoseFocus();
            d.LoseFocus();
        }
        private void UIElement_OnGotFocus3(object sender, RoutedEventArgs e)
        {
            b.LoseFocus();
            a.LoseFocus();
            d.LoseFocus();
        }

        private void UIElement_OnGotFocus4(object sender, RoutedEventArgs e)
        {
            b.LoseFocus();
            a.LoseFocus();
            c.LoseFocus();
        }
    }
}
