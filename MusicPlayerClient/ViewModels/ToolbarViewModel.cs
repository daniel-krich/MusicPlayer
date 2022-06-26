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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayerClient.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private readonly PlaylistStore _playlistStore;
        public ICommand DeletePlaylist { get; }
        public ICommand NavigatePlaylist { get; }
        public ICommand CreatePlaylist { get; }
        public ObservableCollection<PlaylistModel> Playlists { get; set; }

        public ToolbarViewModel(IMusicPlayerService musicPlayerService, INavigationService navigationService, PlaylistBrowserNavigationStore playlistBrowserStore, PlaylistStore playlistStore, MediaStore mediaStore)
        {
            _playlistStore = playlistStore;

            playlistStore.PlaylistNameChanged += OnPlaylistNameChanged;

            Playlists = new ObservableCollection<PlaylistModel>(playlistStore.Playlists.Select(x => new PlaylistModel
            {
                Id = x.Id,
                Name = x.Name,
                CreationDate = x.CreationDate
            }).ToList());

            NavigatePlaylist = new SwitchPageToPlaylistCommand(navigationService, playlistBrowserStore);
            DeletePlaylist = new DeleteSpecificPlaylistCommand(musicPlayerService, navigationService, playlistBrowserStore, playlistStore, mediaStore, Playlists);
            CreatePlaylist = new CreatePlaylistCommand(playlistStore, Playlists);
        }

        private void OnPlaylistNameChanged(object? sender, PlaylistNameChangedEventArgs args)
        {
            var playlist = Playlists.FirstOrDefault(x => x.Id == args.Id);
            if (playlist != null)
            {
                playlist.Name = args.Name;
            }
        }

        public override void Dispose()
        {
            _playlistStore.PlaylistNameChanged -= OnPlaylistNameChanged;
        }
    }
}
