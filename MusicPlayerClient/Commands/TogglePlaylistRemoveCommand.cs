using MusicPlayerClient.Core;
using MusicPlayerClient.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class TogglePlaylistRemoveCommand : CommandBase
    {
        private readonly ObservableWrapper<bool> _isRemoveActive;
        public TogglePlaylistRemoveCommand(ObservableWrapper<bool> isRemoveActive)
        {
            _isRemoveActive = isRemoveActive;
        }

        public override void Execute(object? parameter)
        {
            _isRemoveActive.Object = !_isRemoveActive.Object;
        }
    }
}
