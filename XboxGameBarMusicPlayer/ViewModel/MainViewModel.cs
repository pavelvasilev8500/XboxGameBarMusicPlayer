using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Media.Core;
using Windows.Storage.Pickers;

namespace XboxGameBarMusicPlayer.ViewModel
{
    internal class MainViewModel : ObservableObject
    {

        public IAsyncRelayCommand OpenCommand { get; private set; }
        public IAsyncRelayCommand PlayCommand { get; private set; }

        private MediaSource _musicPath;
        public MediaSource MusicPath
        {
            get => _musicPath;
            private set => SetProperty(ref _musicPath, value);
        }

        public MainViewModel()
        {
            OpenCommand = new AsyncRelayCommand(Open);
            PlayCommand = new AsyncRelayCommand(Play);
        }

        private async Task Play()
        {
            if(ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }

        private async Task Open()
        {
            var fop = new FileOpenPicker();
            fop.ViewMode = PickerViewMode.Thumbnail;
            fop.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            fop.CommitButtonText = "Open";
            fop.FileTypeFilter.Add(".mp3");
            var file = await fop.PickSingleFileAsync();
            if (file != null)
            {
                MusicPath = MediaSource.CreateFromStorageFile(file);
            }
        }
    }
}
