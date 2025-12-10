using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxGameBarBackgroundPlayer
{
    internal static class Log
    {
        private static readonly string _path = Path.Combine(AppContext.BaseDirectory, "log.log");

        public static void AddToLog(string message)
        {
            string msg = $"[{DateTime.Now}] {message}";
            if (!File.Exists(_path))
                File.AppendAllText(_path, msg);
        }
    }
}
