using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XboxGameBarMusicPlayer.ViewModel;

namespace XboxGameBarMusicPlayer
{
    public sealed partial class MainPage : Page
    {
        private bool _isFirst = true;
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
            (DataContext as MainViewModel).InitTrack(item);
        }
    }
}
