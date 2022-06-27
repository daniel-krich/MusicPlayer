using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class SwitchPageToDownloadsCommand : CommandBase
    {
        private readonly INavigationService _navigationService;
        private readonly PlaylistBrowserNavigationStore _playlistBrowserNav;
        public SwitchPageToDownloadsCommand(INavigationService navigationService, PlaylistBrowserNavigationStore playlistBrowserNav)
        {
            _navigationService = navigationService;
            _playlistBrowserNav = playlistBrowserNav;
        }

        public override void Execute(object? parameter)
        {
            _playlistBrowserNav.BrowserPlaylistId = 0;
            _navigationService.NavigateDownloads();
        }
    }
}
