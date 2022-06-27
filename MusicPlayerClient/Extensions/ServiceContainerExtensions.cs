﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using MusicPlayerClient.ViewModels;
using MusicPlayerData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Extensions
{
    public static class ServiceContainerExtensions
    {
        public static IServiceCollection AddViewModels(this IServiceCollection collection)
        {
            collection.AddTransient<HomeViewModel>();
            collection.AddTransient<PlaylistViewModel>();
            collection.AddTransient<DownloadsViewModel>();
            collection.AddSingleton<PlayerViewModel>();
            collection.AddSingleton<ToolbarViewModel>();
            collection.AddSingleton<MainViewModel>();
            return collection;
        }

        public static IServiceCollection AddStores(this IServiceCollection collection)
        {
            collection.AddSingleton<MediaStore>();
            collection.AddSingleton<PlaylistStore>();
            collection.AddSingleton<PlaylistBrowserNavigationStore>();
            return collection;
        }

        public static IServiceCollection AddNavigation(this IServiceCollection collection)
        {
            collection.AddTransient<INavigationService>(s => 
                new NavigationService(
                    () => s.GetRequiredService<MainViewModel>(),
                    () => s.GetRequiredService<HomeViewModel>(),
                    () => s.GetRequiredService<PlaylistViewModel>(),
                    () => s.GetRequiredService<DownloadsViewModel>()
            ));

            return collection;
        }

        public static IServiceCollection AddServices(this IServiceCollection collection)
        {
            collection.AddSingleton<IMusicPlayerService, MusicPlayerService>();
            collection.AddSingleton<IYouTubeClientService, YouTubeClientService>();
            return collection;
        }

        public static IServiceCollection AddDbContextFactory(this IServiceCollection collection)
        {
            collection.AddDbContextFactory<DataContext>();
            return collection;
        }
    }
}
