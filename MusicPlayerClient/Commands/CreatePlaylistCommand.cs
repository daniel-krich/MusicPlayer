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
    public class CreatePlaylistCommand : CommandBase
    {
        private readonly PlaylistStore _playlistStore;
        private readonly ObservableCollection<PlaylistModel>? _observablePlaylists;
        public CreatePlaylistCommand(PlaylistStore playlistStore)
        {
            _playlistStore = playlistStore;
        }

        public CreatePlaylistCommand(PlaylistStore playlistStore, ObservableCollection<PlaylistModel> observablePlaylists) : this(playlistStore)
        {
            _observablePlaylists = observablePlaylists;
        }

        public override void Execute(object? parameter)
        {

            var playlistId = _playlistStore.Playlists.Count() + 1;

            var playlist = new PlaylistEntity
            {
                Name = $"My Playlist #{playlistId}",
                CreationDate = DateTime.Now
            };

            _playlistStore.Add(playlist);

            _observablePlaylists?.Add(new PlaylistModel
            {
                Id = playlist.Id,
                Name = playlist.Name,
                CreationDate = playlist.CreationDate
            });
        }
    }
}
