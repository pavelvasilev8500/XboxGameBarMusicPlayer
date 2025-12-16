using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XboxGameBarMusicPlayer.ViewModel;

namespace XboxGameBarMusicPlayer
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private bool _isSeeking = false;
        public MainPage()
        {
            this.InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, object e)
        {
            if (!_isSeeking)
                (DataContext as MainViewModel).Position = MediaPlayerIUElement.Position.TotalSeconds;
        }

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
            switch (state)
            {
                case MediaElementState.Opening:
                    break;
                case MediaElementState.Buffering:
                    break;
                case MediaElementState.Playing:
                    (DataContext as MainViewModel).TrackSize = MediaPlayerIUElement.NaturalDuration.TimeSpan.TotalSeconds;
                    _timer.Start();
                    break;
                case MediaElementState.Paused:
                    break;
                case MediaElementState.Stopped:
                    _timer.Stop();
                    break;
            }
        }

        private void Slider_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e) => _isSeeking = true;

        private void Slider_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            _isSeeking = false;
            MediaPlayerIUElement.Position = TimeSpan.FromSeconds((DataContext as MainViewModel).Position);
        }

        private void Slider_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _isSeeking = true;
            var slider = (Slider)sender;
            var point = e.GetPosition(slider);
            var ratio = point.X / slider.ActualWidth;
            var newValue = slider.Minimum + ratio * (slider.Maximum - slider.Minimum);
            (DataContext as MainViewModel).Position = newValue;
            MediaPlayerIUElement.Position = TimeSpan.FromSeconds((DataContext as MainViewModel).Position);
            _isSeeking = false;
        }

        private void MediaPlayerIUElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var err = e.ErrorMessage;
        }
    }
}
