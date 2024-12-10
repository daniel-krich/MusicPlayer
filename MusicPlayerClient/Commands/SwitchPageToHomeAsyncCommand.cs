using MusicPlayerClient.Dispachers;
using MusicPlayerClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class SwitchPageToHomeAsyncCommand : AsyncCommandBase
    {
        private readonly INavigationService _navigationService;
        private readonly PlaylistBrowserNavigationDispacher _playlistBrowserNavigationDispacher;
        public SwitchPageToHomeAsyncCommand(INavigationService navigationService, PlaylistBrowserNavigationDispacher playlistBrowserNavigationDispacher)
        {
            _navigationService = navigationService;
            _playlistBrowserNavigationDispacher = playlistBrowserNavigationDispacher;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            _playlistBrowserNavigationDispacher.BrowserPlaylistId = 0;
            await _navigationService.NavigateHome();
        }
    }
}
