using Microsoft.EntityFrameworkCore;
using MusicPlayerClient.Commands;
using MusicPlayerClient.Services;
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
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

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

        public ToolbarViewModel(INavigationService navigationService, IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            GotoPlaylist = new SwitchPageToPlaylistCommand(navigationService);
            GotoHome = new SwitchPageToHomeCommand(navigationService);

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                Playlists = new ObservableCollection<PlaylistEntity>(dbContext.Playlists.ToList());
            }
        }
    }
}
