using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Search;

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
            var folder = KnownFolders.MusicLibrary;
            var query = folder.CreateItemQuery();
            query.ContentsChanged += Query_ContentsChanged;
            Init.Player.IsLoopingEnabled = false;
            PlayPauseCommand = new RelayCommand(PlayPause);
            RepeateCommand = new RelayCommand(Repeate);
            NextTrackCommand = new RelayCommand(Next);
            PreviuseTrackCommand = new RelayCommand(Previuse);
            Scan();
        }

        private void Query_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            Playlist.Clear();
            Scan();
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
        }
    }
}
