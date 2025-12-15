using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Devices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XboxGameBarMusicPlayer.ViewModel;

namespace XboxGameBarMusicPlayer
{
    public sealed partial class MainPage : Page
    {
        //private bool _isFirst = true;
        public MainPage()
        {
            this.InitializeComponent();
        }

        //private void StackPanel_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    if (_isFirst)
        //        List.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        //    _isFirst = false;
        //}

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as PlaylistModel;
            sender.Text = item.Title;
            (DataContext as MainViewModel).InitTrack(item);
        }

        private void MediaPlayerElement_Loaded(object sender, RoutedEventArgs e)
        {
            MediaPlayerIUElement.Source = null;
            (DataContext as MainViewModel).Player = MediaPlayerIUElement;
        }

        private void MediaPlayerIUElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).SelectedItem++;
        }

        private void MediaPlayerIUElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            var dis = this.Dispatcher;
            var state = MediaPlayerIUElement.CurrentState;
            bool isEnd = false;
            switch (state)
            {
                case MediaElementState.Opening:
                    isEnd = false;
                    break;
                case MediaElementState.Buffering:
                    break;
                case MediaElementState.Playing:
                    break;
                case MediaElementState.Paused:
                    break;
                case MediaElementState.Stopped:
                    isEnd = true;
                    break;
            }
        }
    }
}
