using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Jellyfin.Core.Models;

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
    }
}
