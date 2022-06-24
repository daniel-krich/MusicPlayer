using Microsoft.EntityFrameworkCore;
using MusicPlayerClient.Commands;
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
        private string? _name = "toolbar is cool";
        public string? Name
        {
            get { return _name; }
            set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public ICommand GotoPlaylist { get; }
        public ICommand GotoHome { get; }
        public ObservableCollection<PlaylistEntity> Playlists { get; set; }

        public ToolbarViewModel(INavigationService navigationService, PlaylistStore playlistStore, PlaylistBrowserNavigationStore playlistBrowserNavigationStore)
        {
            GotoPlaylist = new SwitchPageToPlaylistCommand(navigationService, playlistBrowserNavigationStore);
            GotoHome = new SwitchPageToHomeCommand(navigationService);

            Playlists = new ObservableCollection<PlaylistEntity>(playlistStore.Playlists.ToList());
        }
    }
}
