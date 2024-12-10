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
        protected override async void OnStartup(StartupEventArgs e)
        {
            IServiceCollection services = new ServiceCollection();

            _serviceProvider = services.AddViewModels()
                                       .AddNavigation()
                                       .AddDbContextFactory()
                                       .AddStores()
                                       .AddServices()
                                       .BuildServiceProvider();

            IDbContextFactory<DataContext> dbFactory = _serviceProvider.GetRequiredService<IDbContextFactory<DataContext>>();

            Directory.CreateDirectory("data"); // Create data directory for db files or temp files.

            using (var dbContext = dbFactory.CreateDbContext())
            {
                dbContext.Database.Migrate();
            }

            ViewModelBase vw = _serviceProvider.GetRequiredService<MainViewModel>();

            MainWindow = new MainWindow()
            {
                DataContext = vw
            };
            MainWindow.Show();
            await vw.InitViewModel();

            base.OnStartup(e);
        }
    }
}