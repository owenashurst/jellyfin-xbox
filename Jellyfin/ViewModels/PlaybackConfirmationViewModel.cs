using System;
using System.Text;
using Windows.System;
using Jellyfin.Models;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class PlaybackConfirmationViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region NextMediaElement

        private MediaElementBase _nextMediaElement;

        public MediaElementBase NextMediaElement
        {
            get { return _nextMediaElement; }
            set
            {
                _nextMediaElement = value;
                RaisePropertyChanged(nameof(NextMediaElement));
            }
        }

        #endregion

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "PlayFromBeginning":
                    PlayFromBeginning(false);
                    break;
                case "PlayFromPosition":
                    PlayFromPosition();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private void PlayFromBeginning(bool isPopupDisplayed)
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = true,
                WasPlaybackPopupShown = isPopupDisplayed,
                NextMediaElement = NextMediaElement
            });
        }

        private void PlayFromPosition()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = false,
                WasPlaybackPopupShown = true,
                NextMediaElement = NextMediaElement
            });
        }

        #endregion

        public bool HandleKeyPressed(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    NavigationService.GoBack();
                    return true;
                default:
                    return false;
            }
        }
    }
}