using MusicPlayerClient.Dispachers;
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
    public class DeleteSpecificPlaylistAsyncCommand : AsyncCommandBase
    {
        private readonly IMusicPlayerService _musicService;
        private readonly PlaylistStore _playlistStore;
        private readonly MediaStore _mediaStore;
        private readonly INavigationService _navigationService;
        private readonly PlaylistBrowserNavigationDispatcher _playlistBrowserDispacher;
        private readonly ObservableCollection<PlaylistModel>? _observablePlaylists;
        public DeleteSpecificPlaylistAsyncCommand(IMusicPlayerService musicService, INavigationService navigationService, PlaylistBrowserNavigationDispatcher playlistBrowserDispacher, PlaylistStore playlistStore, MediaStore mediaStore)
        {
            _musicService = musicService;
            _playlistStore = playlistStore;
            _navigationService = navigationService;
            _playlistBrowserDispacher = playlistBrowserDispacher;
            _mediaStore = mediaStore;
        }

        public DeleteSpecificPlaylistAsyncCommand(IMusicPlayerService musicService, INavigationService navigationService, PlaylistBrowserNavigationDispatcher playlistBrowserDispacher,
                                             PlaylistStore playlistStore, MediaStore mediaStore, ObservableCollection<PlaylistModel> observablePlaylists) : this(musicService, navigationService, playlistBrowserDispacher, playlistStore, mediaStore)
        {
            _observablePlaylists = observablePlaylists;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if (parameter is int playlistId)
            {
                if (_musicService.CurrentMedia?.PlayerlistId == playlistId)
                {
                    _musicService.Stop();
                }

                _observablePlaylists?.RemoveAll(x => x.Id == playlistId);

                await _mediaStore.RemoveAll(x => x.PlayerlistId == playlistId);

                await _playlistStore.Remove(playlistId);

                if (_playlistBrowserDispacher.BrowserPlaylistId == playlistId)
                {
                    await _navigationService.NavigateHome();
                }
            }
        }
    }
}
