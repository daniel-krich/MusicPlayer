using MusicPlayerClient.Core;
using MusicPlayerClient.Services;
using MusicPlayerClient.ViewModels;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class ToggleVolumeCommand : CommandBase
    {
        private readonly PlayerViewModel _player;
        private int _volume;
        public ToggleVolumeCommand(PlayerViewModel player)
        {
            _player = player;
        }

        public override void Execute(object? parameter)
        {
            if(parameter is int volume)
            {
                if(volume == 0)
                {
                    _player.Volume = _volume;
                }
                else
                {
                    _volume = _player.Volume;
                    _player.Volume = 0;
                }
            }
        }
    }
}
