using MusicPlayerClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class SwitchPageToPlaylistCommand : CommandBase
    {
        private readonly INavigationService _navigationService;
        public SwitchPageToPlaylistCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Execute(object? parameter)
        {
            _navigationService.NavigatePlaylist();
        }
    }
}
