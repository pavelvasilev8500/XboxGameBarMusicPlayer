using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace XboxGameBarMusicPlayer.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private ObservableCollection<PlaylistModel> _playlist = new ObservableCollection<PlaylistModel>();
        private ObservableCollection<PlaylistModel> _searchlist = new ObservableCollection<PlaylistModel>();
        private int _selectedItem = -1;
        private string _searchQuery = string.Empty;
        private string _chosenItem = string.Empty;
        private bool _isPlay = false;
        private bool _isFirst = true;
        private int _trackId { get; set; } = 0;

        public RelayCommand PlayPauseCommand { get; private set; }
        public RelayCommand RepeateCommand { get; private set;  }
        public RelayCommand NextTrackCommand { get; private set; }
        public RelayCommand PreviuseTrackCommand { get; private set; }

        public ObservableCollection<PlaylistModel> Playlist
        {
            get => _playlist;
            set => SetProperty(ref _playlist, value);
        }

        public ObservableCollection<PlaylistModel> Searchlist
        {
            get => _searchlist;
            set => SetProperty(ref _searchlist, value);
        }

        public int SelectedItem
        {
            get => _selectedItem;
            set
            {
                int trackCount = _playlist.Count - 1;
                if (_isFirst)
                {
                    _isFirst = false;
                }
                else
                {
                    if (value > trackCount)
                        value = 0;
                    else if (value < 0)
                        value = trackCount;
                }
                SetProperty(ref _selectedItem, value);
                InitTrack();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
                value.ToLower();
                if (!string.IsNullOrEmpty(value))
                    Searchlist = new ObservableCollection<PlaylistModel>(Playlist.Where(t => t.Title.ToLower().Contains(value)));
            }
        }

        public MainViewModel()
        {
            ScanFolders();
            Init.Player.IsLoopingEnabled = false;
            Init.Player.MediaEnded += Player_MediaEndedAsync;
            PlayPauseCommand = new RelayCommand(PlayPause);
            RepeateCommand = new RelayCommand(Repeate);
            NextTrackCommand = new RelayCommand(Next);
            PreviuseTrackCommand = new RelayCommand(Previuse);
            Scan();
        }

        private async void ScanFolders()
        {
            var folder = KnownFolders.MusicLibrary;
            var query = folder.CreateItemQueryWithOptions(
                new Windows.Storage.Search.QueryOptions(
                    Windows.Storage.Search.CommonFileQuery.DefaultQuery, new List<string>() { "*" }));
            query.ContentsChanged += QueryContentsChanged;
            await query.GetItemsAsync();
        }

        private void QueryContentsChanged(IStorageQueryResultBase sender, object args)
        {
            MakeInUI(() =>
            {
                Playlist.Clear();
                Scan();
            });
        }

        private void Player_MediaEndedAsync(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            MakeInUI(Next);
        }

        private async void MakeInUI(Action action)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,() => { action(); });
        }


        private void InitTrack()
        {
            Init.Player.Pause();
            Init.Player.PlaybackSession.Position = TimeSpan.Zero;
            Init.Player.Source = null;
            var source = MediaSource.CreateFromStorageFile(Playlist[SelectedItem].Track);
            Init.Player.Source = source;
            _isPlay = true;
            PlayerControl.Play();
        }

        public void InitTrack(PlaylistModel file)
        {
            Init.Player.Pause();
            Init.Player.PlaybackSession.Position = TimeSpan.Zero;
            Init.Player.Source = null;
            var source = MediaSource.CreateFromStorageFile(file.Track);
            Init.Player.Source = source;
            SelectedItem = file.TrackId;
            _isPlay = true;
            PlayerControl.Play();
        }

        private void PlayPause()
        {
            if (_isPlay)
                PlayerControl.Pause();
            else
                PlayerControl.Play();
            _isPlay = !_isPlay;
        }

        private void Repeate() => PlayerControl.Repeate();

        private void Next() => SelectedItem++;

        private void Previuse() => SelectedItem--;

        private async void Scan() => await ScanFolders(KnownFolders.MusicLibrary);

        private async Task ScanFolders(StorageFolder folder)
        {
            await AddFiles(folder);
            var subfolders = await folder.GetFoldersAsync();
            foreach (var subfolder in subfolders)
            {
                await ScanFolders(subfolder);
            }
        }

        private async Task AddFiles(StorageFolder folder)
        {
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.FileType.Contains("mp3"))
                {
                    _playlist.Add(new PlaylistModel
                    {
                        TrackId = _trackId,
                        Track = file,
                        Title = file.DisplayName,
                        Path = file.Path
                    });
                    _trackId++;
                }
            }
            //var sorted = Playlist.OrderBy(t => t, Comparer<PlaylistModel>.Create((t1, t2) => t1.Title.CompareTo(t2.Title)));
            //foreach(var t in sorted)
            //    Playlist.Add(t);
        }
    }
}
