using Microsoft.EntityFrameworkCore;
using MusicPlayerClient.Commands;
using MusicPlayerClient.Events;
using MusicPlayerClient.Models;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayerClient.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private readonly PlaylistStore _playlistStore;
        private readonly PlaylistBrowserNavigationStore _playlistBrowserStore;
        private readonly IMusicPlayerService _musicPlayerService;

        public ICommand DeletePlaylist { get; }
        public ICommand NavigatePlaylist { get; }
        public ICommand NavigateDownloads { get; }
        public ICommand CreatePlaylist { get; }
        public ObservableCollection<PlaylistModel> Playlists { get; set; }

        public ToolbarViewModel(IMusicPlayerService musicPlayerService, INavigationService navigationService, PlaylistBrowserNavigationStore playlistBrowserStore, PlaylistStore playlistStore, MediaStore mediaStore)
        {
            _playlistStore = playlistStore;

            playlistStore.PlaylistNameChanged += OnPlaylistNameChanged;

            _musicPlayerService = musicPlayerService;

            musicPlayerService.MusicPlayerEvent += OnMusicPlayerEvent;

            Playlists = new ObservableCollection<PlaylistModel>(playlistStore.Playlists.Select(x => new PlaylistModel
            {
                Id = x.Id,
                Name = x.Name,
                CreationDate = x.CreationDate
            }).ToList());

            _playlistBrowserStore = playlistBrowserStore;
            playlistBrowserStore.PlaylistBrowserChanged += OnPlaylistBrowserChanged;

            NavigatePlaylist = new SwitchPageToPlaylistCommand(navigationService, playlistBrowserStore);
            NavigateDownloads = new SwitchPageToDownloadsCommand(navigationService, playlistBrowserStore);
            DeletePlaylist = new DeleteSpecificPlaylistAsyncCommand(musicPlayerService, navigationService, playlistBrowserStore, playlistStore, mediaStore, Playlists);
            CreatePlaylist = new CreatePlaylistAsyncCommand(playlistStore, Playlists);
        }

        private void OnPlaylistNameChanged(object? sender, PlaylistNameChangedEventArgs args)
        {
            var playlist = Playlists.FirstOrDefault(x => x.Id == args.Id);
            if (playlist != null)
            {
                playlist.Name = args.Name;
            }
        }

        private void OnPlaylistBrowserChanged(object? sender, PlaylistBrowserChangedEventArgs args)
        {
            Playlists.ToList().ForEach(x =>
            {
                if (x.Id == args.PlaylistId)
                {
                    x.IsSelected = true;
                }
                else
                {
                    x.IsSelected = false;
                }
            });
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    Playlists.ToList().ForEach(x =>
                    {
                        if (x.Id == e.Media?.PlayerlistId)
                        {
                            x.IsPlaying = true;
                        }
                        else
                        {
                            x.IsPlaying = false;
                        }
                    });
                    break;
                default:
                    Playlists.ToList().ForEach(x => x.IsPlaying = false);
                    break;
            }
        }

        public override void Dispose()
        {
            _playlistStore.PlaylistNameChanged -= OnPlaylistNameChanged;
            _playlistBrowserStore.PlaylistBrowserChanged -= OnPlaylistBrowserChanged;
            _musicPlayerService.MusicPlayerEvent -= OnMusicPlayerEvent;
        }
    }
}
