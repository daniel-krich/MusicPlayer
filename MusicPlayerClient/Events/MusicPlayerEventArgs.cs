using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Events
{
    public enum PlayerEventType
    {
        Paused,
        Playing,
        Stopped
    }

    public class MusicPlayerEventArgs
    {
        public PlayerEventType Type { get; }
        public MediaEntity? Media { get; }

        public MusicPlayerEventArgs(PlayerEventType type, MediaEntity? media)
        {
            Type = type;
            Media = media;
        }
    }
}
