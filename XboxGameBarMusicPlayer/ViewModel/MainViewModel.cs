using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace XboxGameBarMusicPlayer.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private ObservableCollection<PlaylistModel> _playlist = new ObservableCollection<PlaylistModel>();
        private ObservableCollection<PlaylistModel> _searchlist = new ObservableCollection<PlaylistModel>();
        private int _selectedItem = -1;
        private string _searchQuery = string.Empty;
        private bool _isPlay = false;
        private bool _isFirst = true;
        private double _position;
        private double _trackSize;
        private int _trackId { get; set; } = 0;
        public MediaElement Player { get; set; }
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
                InitTrack(null);
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
        public double Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
        public double TrackSize
        {
            get { return _trackSize; }
            set { SetProperty(ref _trackSize, value); }
        }

        public MainViewModel()
        {
            //ScanFolders();
            PlayPauseCommand = new RelayCommand(PlayPause);
            RepeateCommand = new RelayCommand(Repeate);
            NextTrackCommand = new RelayCommand(Next);
            PreviuseTrackCommand = new RelayCommand(Previuse);
            Scan();
        }

        //private async void ScanFolders()
        //{
        //    var folder = KnownFolders.MusicLibrary;
        //    var query = folder.CreateItemQueryWithOptions(
        //        new Windows.Storage.Search.QueryOptions(
        //            Windows.Storage.Search.CommonFileQuery.DefaultQuery, new List<string>() { "*" }));
        //    query.ContentsChanged += QueryContentsChanged;
        //    await query.GetItemsAsync();
        //}

        public async void InitTrack(PlaylistModel file)
        {
            Player.Pause();
            Player.Position = TimeSpan.Zero;
            Player.Source = null;
            StorageFile storageFile = null;
            if(file == null)
            {
                storageFile = await StorageFile.GetFileFromPathAsync(Playlist[SelectedItem].Path);
                Player.SetSource(await storageFile.OpenAsync(FileAccessMode.Read), Playlist[SelectedItem].Track.ContentType);
            }
            else
            {
                storageFile = await StorageFile.GetFileFromPathAsync(file.Path);
                Player.SetSource(await storageFile.OpenAsync(FileAccessMode.Read), file.Track.ContentType);
                SelectedItem = file.TrackId;
            }
            _isPlay = true;
            Player.Play();
        }

        private void PlayPause()
        {
            if (_isPlay)
                Player.Pause();
            else
                Player.Play();
            _isPlay = !_isPlay;
        }

        private void Repeate()
        {
            Player.IsLooping = !Player.IsLooping;
        }

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
