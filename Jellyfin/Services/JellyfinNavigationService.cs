using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public sealed class JellyfinNavigationService : IJellyfinNavigationService
    {
        public void Navigate(Type sourcePage)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, null, new DrillInNavigationTransitionInfo());
        }

        public void Navigate(Type sourcePage, object parameter)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, parameter, new DrillInNavigationTransitionInfo());
        }

        public void Navigate(string sourcePage)
        {
            Navigate(Type.GetType(sourcePage));
        }
        public void Navigate(string sourcePage, object parameter)
        {
            Navigate(Type.GetType(sourcePage), parameter);
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public void GoForward()
        {
            // Frame.CanGoForward()?
            Go(true);
        }
        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public void GoBack()
        {
            // Frame.CanGoBack()?
            Go(false);
        }

        public Type GetPreviousPage()
        {
            Frame frame = (Frame)Window.Current.Content;

            var backStack = frame.BackStack.ToList();
            return backStack.LastOrDefault().SourcePageType;
        }

        private static void Go(bool isForward)
        {
            Frame frame = (Frame)Window.Current.Content;
            if (isForward)
            {
                frame.GoForward();
            }
            else
            {
                frame.GoBack();
            }
        }
    }
}
