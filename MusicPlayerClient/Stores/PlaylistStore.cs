using Microsoft.EntityFrameworkCore;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using MusicPlayerClient.Events;

namespace MusicPlayerClient.Stores
{
    public class PlaylistStore
    {
        public event EventHandler<PlaylistNameChangedEventArgs>? PlaylistNameChanged;

        private readonly List<PlaylistEntity> _playlists;
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public IEnumerable<PlaylistEntity> Playlists => _playlists;

        public PlaylistStore(IDbContextFactory<DataContext> dbContextFactory)
        {
            _playlists = new List<PlaylistEntity>();
            _dbContextFactory = dbContextFactory;
            Load();
        }

        public void Load()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                _playlists.AddRange(dbContext.Playlists.ToList());
            }
        }

        public void Rename(int playlistId, string name)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var dbPlaylist = dbContext.Playlists.Find(playlistId);
                if (dbPlaylist != null)
                {
                    dbPlaylist.Name = name;
                    dbContext.SaveChanges();

                    var playlist = _playlists.FirstOrDefault(x => x.Id == playlistId);
                    if (playlist != null)
                    {
                        playlist.Name = name;
                    }

                    PlaylistNameChanged?.Invoke(this, new PlaylistNameChangedEventArgs(playlistId, name));
                }
            }
        }

        public bool Add(PlaylistEntity playlistEntity)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    dbContext.Playlists.Add(playlistEntity);
                    dbContext.SaveChanges();

                    _playlists.Add(playlistEntity);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public bool Remove(int playlistId)
        {
            _playlists.RemoveAll(x => x.Id == playlistId);
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    dbContext.Playlists.Remove(new PlaylistEntity { Id = playlistId });
                    dbContext.SaveChanges();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
