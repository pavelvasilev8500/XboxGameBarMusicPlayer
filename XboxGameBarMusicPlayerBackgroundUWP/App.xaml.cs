using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Media.Playback;
using Windows.UI.Xaml;

namespace XboxGameBarMusicPlayerBackgroundUWP
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await BackgroundExecutionManager.RequestAccessAsync();
            var bgPlayer = BackgroundMediaPlayer.Current;
            var valueSet = new Windows.Foundation.Collections.ValueSet { { "cmd", "init" } };
            BackgroundMediaPlayer.SendMessageToBackground(valueSet);

        }
    }
}
