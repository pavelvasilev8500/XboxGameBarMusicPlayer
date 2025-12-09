using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XboxGameBarMusicPlayerBackground
{

    internal class Program
    {
        private static readonly int _port = 5051;
        private static TcpListener _server = new TcpListener(IPAddress.Loopback, _port);

        private static string _path = string.Empty;
        private static List<string> _playlist;

        private static WaveOutEvent _outDevice = new WaveOutEvent();
        private static AudioFileReader _fileReader;

        private static int _currentTrackIndex = 0;

        static async Task Main(string[] args)
        {
            _outDevice.PlaybackStopped += _outDevice_PlaybackStopped;
            await StartServer();
        }

        private static async Task StartServer()
        {
            try
            {
                Log.AddToLog("Server started");
                _server.Start();
                while (true)
                {
                    Log.AddToLog("Wait for clients...");
                    var client = await _server.AcceptTcpClientAsync();
                    var msg = await GetMessage(client.GetStream(), new byte[1024], 0);
                    Control(msg);
                }
            }
            catch (Exception ex)
            {
                Log.AddToLog(ex.ToString());
                Log.AddToLog("Server restarted...");
                Log.AddToLog("Server stoped");
                _server.Stop();
                Log.AddToLog("Trying start server...");
                await StartServer();
            }
        }

        private static async Task<string> GetMessage(NetworkStream stream,byte[] data, int bytes)
        {
            Log.AddToLog("Recive message");
            var msg = new StringBuilder();
            do
            {
                bytes = await stream.ReadAsync(data, 0, data.Length);
                msg.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (bytes > 0 && stream.DataAvailable);
            Log.AddToLog("Message recived");
            return msg.ToString();
        }

        private static void Control(string msg)
        {
            try
            {
                bool isSingle = true;
                Log.AddToLog("Trying deserealize message...");
                var message = JsonConvert.DeserializeObject<MessageModel>(msg);
                if(message != null)
                {
                    if (message.Playlist == null && string.IsNullOrEmpty(message.Path))
                        throw new Exception("No music to play");
                    if (message.Playlist != null)
                    {
                        _playlist = new List<string>(message.Playlist);
                        Log.AddToLog("Playlist recived");
                        isSingle = false;
                    }
                    if (!string.IsNullOrEmpty(message.Path))
                    {
                        _path = message.Path;
                        Log.AddToLog("Song recived");
                        isSingle = true;
                    }
                    switch(message.Contol)
                    {
                        case Controls.PLay:
                            if(isSingle)
                                PLay(_path);
                            else
                            {
                                _currentTrackIndex = 0;
                                PLay(_playlist[0]);
                            }
                            break;
                        case Controls.Pause:
                            Pause();
                            break;
                        case Controls.Stop:
                            Stop();
                            break;
                        case Controls.Replay:
                            break;
                        case Controls.Next:
                            break;
                        case Controls.Preview:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AddToLog(ex.ToString());
            }
        }

        private static void _outDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception == null)
            {
                _currentTrackIndex++;
                if(_currentTrackIndex == _playlist.Count)
                    _currentTrackIndex = 0;
                PLay(_playlist[_currentTrackIndex]);
            }
            else
                Log.AddToLog(e.ToString());
        }

        private static void PLay(string path)
        {
            Stop();
            _fileReader = new AudioFileReader(path);
            _outDevice.Init(_fileReader);
            _outDevice.Play();
        }

        private static void Pause()
        {
            _outDevice?.Pause();
        }

        private static void Resume()
        {
            _outDevice?.Play();
        }

        private static void Stop()
        {
            _outDevice?.Stop();
            _fileReader?.Dispose();
            _fileReader = null;
        }

    }
}
