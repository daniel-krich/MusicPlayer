using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayerClient.Extensions;
using MusicPlayerClient.Services;
using MusicPlayerClient.ViewModels;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayerClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        protected override void OnStartup(StartupEventArgs e)
        {
            IServiceCollection services = new ServiceCollection();

            Directory.CreateDirectory("data"); // Create data directory for db files or temp files.

            _serviceProvider = services.AddViewModels()
                                       .AddNavigation()
                                       .AddDbContextFactory()
                                       .AddMusicService()
                                       .BuildServiceProvider();

            IDbContextFactory<DataContext> dbFactory = _serviceProvider.GetRequiredService<IDbContextFactory<DataContext>>();

            using (var dbContext = dbFactory.CreateDbContext())
            {
                dbContext.Database.Migrate();

                /*var playlist = new PlaylistEntity { Name = "My first playlist" };
                var song = new MediaEntity { FilePath = @"D:\Disclosure_Latch.mp3" };
                dbContext.Playlists.Add(playlist);
                dbContext.Songs.Add(song);
                dbContext.SaveChanges();

                dbContext.SongsPlaylists.Add(new MediaPlaylistEntity
                {
                    MediaId = song.Id,
                    PlayerlistId = playlist.Id
                });
                dbContext.SaveChanges();*/
            }

            MainWindow = new MainWindow()
            {
                DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
            };
            MainWindow.Show();

            IMusicPlayerService player = _serviceProvider.GetRequiredService<IMusicPlayerService>();
            //player.Play(6);

            base.OnStartup(e);
        }
    }
}
