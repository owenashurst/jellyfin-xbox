﻿using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    public partial class MovieDetailView
    {
        public MovieDetailView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles key press for the UI.
        /// </summary>
        private void MovieDetailView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as MovieDetailViewModel).HandleKeyPressed(e.Key))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Retrieves the passed movie parameter from the old view.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Movie movie = e.Parameter as Movie;
            if (movie != null)
            {
                (DataContext as MovieDetailViewModel).GetMovieDetails(movie);
            }
        }
    }
}
