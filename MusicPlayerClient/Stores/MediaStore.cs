using Microsoft.EntityFrameworkCore;
using MusicPlayerClient.Models;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Stores
{
    public class MediaStore
    {
        private readonly List<MediaEntity> _songs;
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public IEnumerable<MediaEntity> Songs => _songs;

        public MediaStore(IDbContextFactory<DataContext> dbContextFactory)
        {
            _songs = new List<MediaEntity>();
            _dbContextFactory = dbContextFactory;
            Load();
        }

        public void Load()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                _songs.AddRange(dbContext.Songs.ToList());
            }
        }

        public bool Add(MediaEntity media)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    dbContext.Songs.Add(media);
                    dbContext.SaveChanges();

                    _songs.Add(media);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public bool AddRange(IEnumerable<MediaEntity> medias)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    dbContext.Songs.AddRange(medias);
                    dbContext.SaveChanges();

                    _songs.AddRange(medias);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public bool RemoveAll(Func<MediaEntity, bool> predicate)
        {
            _songs.RemoveAll(x => predicate.Invoke(x));
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    var itemsRemove = dbContext.Songs.Where(predicate);
                    dbContext.Songs.RemoveRange(itemsRemove);
                    dbContext.SaveChanges();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public bool Remove(int mediaId)
        {
            _songs.RemoveAll(x => x.Id == mediaId);
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    dbContext.Songs.Remove(new MediaEntity { Id = mediaId });
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
