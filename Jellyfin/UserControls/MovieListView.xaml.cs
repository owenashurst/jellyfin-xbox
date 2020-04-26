using System.Collections.Generic;
using System.Timers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Jellyfin.UserControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MovieListView
    {
        private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();

        public MovieListView()
        {
            InitializeComponent();
        }
        
        private void MovieGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (DataContext as MovieListViewModel).NavigateToMovie(e.ClickedItem as Movie);
        }

        private void MovieView_OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MovieListViewModel).Load();
        }

        /// <summary>
        /// TODO smurancsik: the speed sux, currently this scrolling is disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            return;
            ScrollViewer scrollViewer = (sender as ScrollViewer);
            if (scrollViewer == null)
            {
                return;
            }

            if (scrollViewer.ScrollableWidth == 0)
            {
                return;
            }

            Timer timer = new Timer();
            timers.Add(scrollViewer.GetHashCode().ToString(), timer);

            timer.Elapsed += (ss, ee) =>
            {
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    if (scrollViewer.HorizontalOffset + 5 >= scrollViewer.ScrollableWidth)
                    {
                        timer.Interval = 3000;
                    }
                    else
                    {
                        timer.Interval = 100;
                    }
                    //each time set the offset to scrollviewer.HorizontalOffset + 5
                    scrollViewer.ChangeView(scrollViewer.HorizontalOffset + 5, null, null);
                    
                    //if the scrollviewer scrolls to the end, scroll it back to the start.
                    if (scrollViewer.HorizontalOffset == scrollViewer.ScrollableWidth)
                    {
                        scrollViewer.ChangeView(0, null, null);
                    }
                });
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            };
            timer.Interval = 100;
            timer.Start();
        }

        private void scrollviewer_Unloaded(object sender, RoutedEventArgs e)
        {
            return;
            timers.Clear();
        }
    }
}
