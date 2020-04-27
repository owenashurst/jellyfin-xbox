using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Jellyfin.UserControls
{
    public partial class MovieItemUserControl
    {
        #region Properties

        private Storyboard animateStoryboard
        {
            get => containerCanvas.Resources["Storyboard1"] as Storyboard;
        }

        #endregion

        #region ctor

        public MovieItemUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Additional methods

        public void StartAnimation()
        {
            Animate(containerCanvas);
            ImageBorder.Background = new SolidColorBrush(Color.FromArgb(255, 0, 164, 220));
        }

        public void EndAnimation()
        {
            animateStoryboard.Stop();
            ImageBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            containerCanvas.Visibility = Visibility.Collapsed;
            notMovingTextBlock.Visibility = Visibility.Visible;
        }
        
        private void Animate(Canvas c)
        {
            if (c.DataContext == null)
            {
                return;
            }
            
            movingTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            movingTextBlock.Arrange(new Rect(new Point(0, 0), movingTextBlock.DesiredSize));
            
            // So the text is longer than the canvas width
            // c.Width = NaN sometimes
            if (movingTextBlock.ActualWidth > 230)
            {
                DoubleAnimation doubleAnimation = animateStoryboard.Children[0] as DoubleAnimation;
                doubleAnimation.To = -1 * movingTextBlock.ActualWidth;

                animateStoryboard.Begin();

                c.Visibility = Visibility.Visible;
                notMovingTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                c.Visibility = Visibility.Collapsed;
                notMovingTextBlock.Visibility = Visibility.Visible;
            }
        }

        #endregion
    }
}
