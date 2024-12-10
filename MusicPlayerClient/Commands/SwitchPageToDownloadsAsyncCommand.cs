using MusicPlayerClient.Dispachers;
using MusicPlayerClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class SwitchPageToDownloadsAsyncCommand : AsyncCommandBase
    {
        private readonly INavigationService _navigationService;
        private readonly PlaylistBrowserNavigationDispatcher _playlistBrowserDispacher;
        public SwitchPageToDownloadsAsyncCommand(INavigationService navigationService, PlaylistBrowserNavigationDispatcher playlistBrowserDispacher)
        {
            _navigationService = navigationService;
            _playlistBrowserDispacher = playlistBrowserDispacher;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            _playlistBrowserDispacher.BrowserPlaylistId = 0;
            await _navigationService.NavigateDownloads();
        }
    }
}
