using MusicPlayerClient.Dispachers;
using MusicPlayerClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class SwitchPageToPlaylistAsyncCommand : AsyncCommandBase
    {
        private readonly INavigationService _navigationService;
        private readonly PlaylistBrowserNavigationDispacher _playlistBrowserNavigationDispacher;
        public SwitchPageToPlaylistAsyncCommand(INavigationService navigationService, PlaylistBrowserNavigationDispacher playlistBrowserNavigationDispacher)
        {
            _navigationService = navigationService;
            _playlistBrowserNavigationDispacher = playlistBrowserNavigationDispacher;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if (parameter is int playlistId)
            {
                if (_playlistBrowserNavigationDispacher.BrowserPlaylistId != playlistId)
                {
                    _playlistBrowserNavigationDispacher.BrowserPlaylistId = playlistId;
                    await _navigationService.NavigatePlaylist();
                }
            }
        }
    }
}
