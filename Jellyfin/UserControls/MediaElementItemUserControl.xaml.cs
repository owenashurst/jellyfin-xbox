using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Jellyfin.Models;

namespace Jellyfin.UserControls
{
    public partial class MediaElementItemUserControl
    {
        #region IsLong Dependency Property

        public static readonly DependencyProperty IsLongDependency = DependencyProperty.Register("IsLong", typeof(bool), typeof(MediaElementItemUserControl), new PropertyMetadata(false, WideChanged));
        
        public bool IsLong
        {
            get => (bool)GetValue(IsLongDependency);
            set => SetValue(IsLongDependency, value);
        }

        #endregion

        #region BlockHeight Dependency Property

        public static readonly DependencyProperty BlockHeightDependency = DependencyProperty.Register("BlockHeight", typeof(int), typeof(MediaElementItemUserControl), new PropertyMetadata(0, SizeChanged));

        public int BlockHeight
        {
            get => (int)GetValue(BlockHeightDependency);
            set => SetValue(BlockHeightDependency, value);
        }

        #endregion

        #region BlockWidth Dependency Property

        public static readonly DependencyProperty BlockWidthDependency = DependencyProperty.Register("BlockWidth", typeof(int), typeof(MediaElementItemUserControl), new PropertyMetadata(0, SizeChanged));

        public int BlockWidth
        {
            get => (int)GetValue(BlockWidthDependency);
            set => SetValue(BlockWidthDependency, value);
        }

        #endregion

        #region IsShowSeriesNameAsSecondLine Dependency Property

        public static readonly DependencyProperty IsShowSeriesNameAsSecondLineDependency = DependencyProperty.Register("IsShowSeriesNameAsSecondLine", typeof(bool), typeof(MediaElementItemUserControl), new PropertyMetadata(false));

        public bool IsShowSeriesNameAsSecondLine
        {
            get => (bool)GetValue(IsShowSeriesNameAsSecondLineDependency);
            set => SetValue(IsShowSeriesNameAsSecondLineDependency, value);
        }

        #endregion

        #region IsProgressBarVisible Dependency Property

        public static readonly DependencyProperty IsProgressBarVisibleDependency = DependencyProperty.Register("IsProgressBarVisible", typeof(bool), typeof(MediaElementItemUserControl), new PropertyMetadata(false));

        public bool IsProgressBarVisible
        {
            get => (bool)GetValue(IsProgressBarVisibleDependency);
            set => SetValue(IsProgressBarVisibleDependency, value);
        }

        #endregion

        #region Properties

        private Storyboard animateStoryboard
        {
            get => containerCanvas.Resources["Storyboard1"] as Storyboard;
        }

        #endregion

        #region ctor

        public MediaElementItemUserControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        #endregion

        #region Additional methods

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            MediaElementBase b = e.NewValue as MediaElementBase;

            if (b == null)
            {
                return;
            }

            if (b.PlaybackPosition.Ticks == 0)
            {
                watchedProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private static void WideChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            //if(e.NewValue != null && (bool)e.NewValue)
            //{
            //    MediaElementItemUserControl mediaElementItemUserControl =
            //        dependencyObject as MediaElementItemUserControl;
            //    mediaElementItemUserControl?.SetWide();
            //}
        }

        private static void SizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            MediaElementItemUserControl mediaElementItemUserControl =
                dependencyObject as MediaElementItemUserControl;

            mediaElementItemUserControl.SetBlockSize();
        }

        public void SetBlockSize()
        {
            var height = BlockHeight;
            var width = BlockWidth;

            if (height < 50)
            {
                return;
            }

            if (width < 50)
            {
                return;
            }

            stackPanel.Width = width;
            DoubleAnimation doubleAnimation = animateStoryboard.Children[0] as DoubleAnimation;
            doubleAnimation.From = width;
            RectangleGeometry.Rect = new Rect(0, 0, width, 40);
        }

        public void FocusGot()
        {
            Animate(containerCanvas);
            ImageBorder.Background = new SolidColorBrush(Color.FromArgb(255, 0, 164, 220));

            notMovingTextBlock.Opacity = 1;
            yearTextBlock.Opacity = 1;
            seriesNameTextBlock.Opacity = 1;
        }

        public void FocusLost()
        {
            animateStoryboard.Stop();
            ImageBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            containerCanvas.Visibility = Visibility.Collapsed;
            notMovingTextBlock.Visibility = Visibility.Visible;

            notMovingTextBlock.Opacity = .7;
            seriesNameTextBlock.Opacity = .7;

            if (IsLong)
            {
                watchedProgressBar.Margin = new Thickness(17, -20, 17, 0);
            }
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

            int limit = IsLong ? 520 : 230;

            if (movingTextBlock.ActualWidth > limit)
            {
                DoubleAnimation doubleAnimation = animateStoryboard.Children[0] as DoubleAnimation;
                doubleAnimation.To = -1 * movingTextBlock.ActualWidth;

                animateStoryboard.Begin();

                c.Visibility = Visibility.Visible;
                notMovingTextBlock.Visibility = Visibility.Collapsed;

                if (IsLong)
                {
                    watchedProgressBar.Margin = new Thickness(17, -100, 17, 0);
                }
            }
            else
            {
                c.Visibility = Visibility.Collapsed;
                notMovingTextBlock.Visibility = Visibility.Visible;
            }
        }

        #endregion

        private void NotMovingTextBlock_OnLoaded(object sender, RoutedEventArgs e)
        {
            //TextBlock tb = sender as TextBlock;
            //if (tb.Width > 
        }
    }
}
