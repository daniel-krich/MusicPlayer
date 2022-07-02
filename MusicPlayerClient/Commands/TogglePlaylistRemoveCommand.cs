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
    public class TogglePlaylistRemoveCommand : CommandBase
    {
        private readonly ToolbarViewModel _toolbar;
        public TogglePlaylistRemoveCommand(ToolbarViewModel toolbar)
        {
            _toolbar = toolbar;
        }

        public override void Execute(object? parameter)
        {
            _toolbar.IsRemoveActive = !_toolbar.IsRemoveActive;
        }
    }
}
