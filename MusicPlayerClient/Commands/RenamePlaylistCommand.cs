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
    public class RenamePlaylistCommand : CommandBase
    {
        private readonly PlaylistStore _playlistStore;
        private readonly PlaylistBrowserNavigationStore _playlistBrowserNavigationStore;

        public RenamePlaylistCommand(PlaylistStore playlistStore, PlaylistBrowserNavigationStore playlistBrowserNavigationStore)
        {
            _playlistStore = playlistStore;
            _playlistBrowserNavigationStore = playlistBrowserNavigationStore;
        }

        public override void Execute(object? parameter)
        {

            if(parameter is string playlistName)
            {
                _playlistStore.Rename(_playlistBrowserNavigationStore.BrowserPlaylistId, playlistName);
            }
        }
    }
}
