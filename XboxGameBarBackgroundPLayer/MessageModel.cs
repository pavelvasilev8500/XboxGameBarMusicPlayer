using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxGameBarBackgroundPlayer
{
    internal class MessageModel
    {
        public string Path { get; set; }
        public List<string> Playlist { get; set; }
        public Controls Contol { get; set; }
    }
}
