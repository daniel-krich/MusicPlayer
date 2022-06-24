using Microsoft.EntityFrameworkCore;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicPlayerClient.Stores
{
    public class PlaylistStore
    {
        private readonly List<PlaylistEntity> _playlists;
        private readonly IDbContextFactory<DataContext> _dbContextFactory;
        //private readonly Lazy<Task> _lazyInitialize;

        public IEnumerable<PlaylistEntity> Playlists => _playlists;

        public PlaylistStore(IDbContextFactory<DataContext> dbContextFactory)
        {
            _playlists = new List<PlaylistEntity>();
            _dbContextFactory = dbContextFactory;
            /*_lazyInitialize = new Lazy<Task>(Initialize);
            Load().Wait();*/
            Load();
        }

        /*public async Task Load()
        {
            await _lazyInitialize.Value;
        }*/

        /*public async Task Initialize()
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                _playlists.AddRange(await dbContext.Playlists.ToListAsync());
            }
        }*/

        public void Load()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                _playlists.AddRange(dbContext.Playlists.ToList());
            }
        }
    }
}
