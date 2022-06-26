using MusicPlayerData.DataEntities;
using NAudio.Wave;
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
        Stopped,
        Finished
    }

    public class MusicPlayerEventArgs
    {
        public PlayerEventType Type { get; }
        public MediaEntity? Media { get; }
        public IWaveProvider? Audio { get; }

        public MusicPlayerEventArgs(PlayerEventType type, MediaEntity? media, IWaveProvider? audio)
        {
            Type = type;
            Media = media;
            Audio = audio;
        }
    }
}
