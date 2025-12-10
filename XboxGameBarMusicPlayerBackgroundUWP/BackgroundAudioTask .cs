using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace XboxGameBarMusicPlayerBackgroundUWP
{
    class BackgroundAudioTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private MediaPlayer _player;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Log("Run Background");
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;
            _player = BackgroundMediaPlayer.Current;
            _player.MediaEnded += _player_MediaEnded;
            Log("Play from run");
            Play();
        }

        void Log(string s)
        {
            Debug.WriteLine(s);
            File.AppendAllText(ApplicationData.Current.LocalFolder.Path + "\\log.txt",
                DateTime.Now + " " + s + "\n");
        }

        private void _player_MediaEnded(MediaPlayer sender, object args)
        {
            Log("End");
        }

        private async void Play()
        {
            Log("Play in async play");
            await PlayTrack();
        }

        private async Task PlayTrack()
        {
            Log("PlayTrack");
            try
            {
                Log("Get file");
                var file = await KnownFolders.MusicLibrary.GetFileAsync("div2.mp3");
                Log($"File: {file.Path} & Create Source");
                _player.Source = MediaSource.CreateFromStorageFile(file);
                Log("PLay");
                _player.Play();
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _player?.Dispose();
            _deferral.Complete();
        }
    }
}
