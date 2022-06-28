﻿using Microsoft.EntityFrameworkCore;
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

        public async Task Rename(int playlistId, string name)
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                var dbPlaylist = await dbContext.Playlists.FindAsync(playlistId);
                if (dbPlaylist != null)
                {
                    dbPlaylist.Name = name;
                    await dbContext.SaveChangesAsync();

                    var playlist = _playlists.FirstOrDefault(x => x.Id == playlistId);
                    if (playlist != null)
                    {
                        playlist.Name = name;
                    }

                    PlaylistNameChanged?.Invoke(this, new PlaylistNameChangedEventArgs(playlistId, name));
                }
            }
        }

        public async Task<bool> Add(PlaylistEntity playlistEntity)
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                try
                {
                    dbContext.Playlists.Add(playlistEntity);
                    await dbContext.SaveChangesAsync();

                    _playlists.Add(playlistEntity);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Remove(int playlistId)
        {
            _playlists.RemoveAll(x => x.Id == playlistId);
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                try
                {
                    dbContext.Playlists.Remove(new PlaylistEntity { Id = playlistId });
                    await dbContext.SaveChangesAsync();
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
