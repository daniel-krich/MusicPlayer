using MusicPlayerClient.Extensions;
using MusicPlayerClient.Models;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class DeleteSpecificPlaylistCommand : CommandBase
    {
        private readonly IMusicPlayerService _musicService;
        private readonly PlaylistStore _playlistStore;
        private readonly MediaStore _mediaStore;
        private readonly INavigationService _navigationService;
        private readonly PlaylistBrowserNavigationStore _playlistBrowserStore;
        private readonly ObservableCollection<PlaylistModel>? _observablePlaylists;
        public DeleteSpecificPlaylistCommand(IMusicPlayerService musicService, INavigationService navigationService, PlaylistBrowserNavigationStore playlistBrowserStore, PlaylistStore playlistStore, MediaStore mediaStore)
        {
            _musicService = musicService;
            _playlistStore = playlistStore;
            _navigationService = navigationService;
            _playlistBrowserStore = playlistBrowserStore;
            _mediaStore = mediaStore;
        }

        public DeleteSpecificPlaylistCommand(IMusicPlayerService musicService, INavigationService navigationService, PlaylistBrowserNavigationStore playlistBrowserStore,
                                             PlaylistStore playlistStore, MediaStore mediaStore, ObservableCollection<PlaylistModel> observablePlaylists) : this(musicService, navigationService, playlistBrowserStore, playlistStore, mediaStore)
        {
            _observablePlaylists = observablePlaylists;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is int playlistId)
            {
                if (_musicService.CurrentMedia?.PlayerlistId == playlistId)
                {
                    _musicService.Stop();
                }

                _observablePlaylists?.RemoveAll(x => x.Id == playlistId);

                _mediaStore.RemoveAll(x => x.PlayerlistId == playlistId);

                _playlistStore.Remove(playlistId);

                if (_playlistBrowserStore.BrowserPlaylistId == playlistId)
                {
                    _navigationService.NavigateHome();
                }
            }
        }
    }
}
