using MusicPlayerClient.Dispachers;
using MusicPlayerClient.Extensions;
using MusicPlayerClient.Models;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using MusicPlayerData.DataEntities;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class RenamePlaylistAsyncCommand : AsyncCommandBase
    {
        private readonly PlaylistStore _playlistStore;
        private readonly PlaylistBrowserNavigationDispatcher _playlistBrowserNavigationDispacher;

        public RenamePlaylistAsyncCommand(PlaylistStore playlistStore, PlaylistBrowserNavigationDispatcher playlistBrowserNavigationDispacher)
        {
            _playlistStore = playlistStore;
            _playlistBrowserNavigationDispacher = playlistBrowserNavigationDispacher;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {

            if(parameter is string playlistName)
            {
                await _playlistStore.Rename(_playlistBrowserNavigationDispacher.BrowserPlaylistId, playlistName);
            }
        }
    }
}
