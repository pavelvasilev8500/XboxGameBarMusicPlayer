using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XboxGameBarBackgroundPlayer
{
    internal class Program
    {
        private static readonly int _port = 5051;
        private static TcpListener _server = new TcpListener(IPAddress.Loopback, _port);

        private static string _path = string.Empty;
        private static List<string> _playlist;

        private static int _currentTrackIndex = 0;

        static async Task Main(string[] args)
        {
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

        private static async Task<string> GetMessage(NetworkStream stream, byte[] data, int bytes)
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
                Log.AddToLog("Trying deserealize message...");
                var message = JsonConvert.DeserializeObject<MessageModel>(msg);
                if (message != null)
                {
                    if (message.Playlist == null && string.IsNullOrEmpty(message.Path))
                        throw new Exception("No music to play");
                    if (message.Playlist != null)
                    {
                        _playlist = new List<string>(message.Playlist);
                        Log.AddToLog("Playlist recived");
                    }
                    if (!string.IsNullOrEmpty(message.Path))
                    {
                        _playlist = new List<string> { message.Path };
                        Log.AddToLog("Song recived");
                    }
                    switch (message.Contol)
                    {
                        case Controls.PLay:
                            Play();
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

        private static void Play()
        {
        }

        private static void Pause()
        {
        }

        private static void Resume()
        {
        }

        private static void Stop()
        {
        }

        private static void Next()
        {
            _currentTrackIndex++;
            if (_currentTrackIndex == _playlist.Count)
                _currentTrackIndex = 0;
            Play();
        }

        private static void Previous()
        {
            _currentTrackIndex--;
            if (_currentTrackIndex == -1)
                _currentTrackIndex = _playlist.Count;
            Play();
        }
    }
}
