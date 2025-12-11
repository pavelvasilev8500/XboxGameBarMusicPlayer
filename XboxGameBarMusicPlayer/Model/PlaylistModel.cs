using Windows.Storage;

namespace XboxGameBarMusicPlayer
{
    internal class PlaylistModel
    {
        public int TrackId { get; set; }
        public StorageFile Track { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
    }
}
