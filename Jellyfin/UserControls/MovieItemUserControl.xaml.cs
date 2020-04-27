using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Jellyfin.UserControls
{
    public sealed partial class MovieItemUserControl
    {
        public MovieItemUserControl()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Canvas c = sender as Canvas;
            Animate(c);
        }

        private void FrameworkElement_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Canvas c = (sender as Canvas);
            Animate(c);
        }

        private void Animate(Canvas c)
        {
            if (c.DataContext == null)
            {
                return;
            }

            TextBlock textInside = c.Children[0] as TextBlock;

            textInside.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textInside.Arrange(new Rect(new Point(0, 0), textInside.DesiredSize));

            StackPanel parent = (c.Parent as StackPanel);
            TextBlock followingTextBox = parent?.Children[2] as TextBlock;

            // So the text is longer than the canvas width
            if (textInside.ActualWidth > 230)
            {
                Storyboard storyboard = c.Resources["Storyboard1"] as Storyboard;
                DoubleAnimation doubleAnimation = (storyboard.Children[0] as DoubleAnimation);
                doubleAnimation.To = -1 * textInside.ActualWidth;

                storyboard.Begin();

                c.Visibility = Visibility.Visible;

                if (followingTextBox != null)
                {
                    followingTextBox.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                c.Visibility = Visibility.Collapsed;

                if (followingTextBox != null)
                {
                    followingTextBox.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
