using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Extensions
{
    public static class AudioUtills
    {
        public static string DurationParse(string? music_file_path)
        {
            try
            {
                SoundPlayer soundPlayer = new SoundPlayer();
                soundPlayer.LoadAsync();
                using (var audioFile = new MediaFoundationReader(music_file_path))
                {
                    var durationParse = $"{Math.Floor(audioFile.TotalTime.TotalSeconds / 60).ToString().PadLeft(2, '0')}:{Math.Floor(audioFile.TotalTime.TotalSeconds % 60).ToString().PadLeft(2, '0')}";
                    return durationParse;
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string DurationParse(long sec)
        {
            return $"{(sec / 60).ToString().PadLeft(2, '0')}:{(sec % 60).ToString().PadLeft(2, '0')}";
        }
    }
}
