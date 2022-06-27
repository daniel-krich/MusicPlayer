using Microsoft.EntityFrameworkCore;
using MusicPlayerClient.Commands;
using MusicPlayerClient.Services;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using MusicPlayerClient.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayerClient.Events;
using System.Diagnostics;
using MusicPlayerClient.Stores;
using MusicPlayerClient.Interfaces;
using System.Windows;
using MusicPlayerClient.Extensions;
using MusicPlayerClient.Core;
using MusicDownloader.Models;

namespace MusicPlayerClient.ViewModels
{
    public class DownloadsViewModel : ViewModelBase
    {
        private readonly IMusicPlayerService _musicService;
        private readonly MediaStore _mediaStore;
        public string CurrentDateString { get; }
        public ObservableCollection<YoutubeVideoInfoModel> ResultMedia { get; }
        public ObjectWrapper<bool> LoadingWrapper { get; }
        public ObjectWrapper<bool> FailedWrapper { get; }
        public ICommand? SearchMedia { get; set; }
        public ICommand NavigateHome { get; }
        public ICommand DownloadMedia { get; }

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
        
        public DownloadsViewModel(MediaStore mediaStore, IMusicPlayerService musicService, IYouTubeClientService youtubeClient, INavigationService navigationService, PlaylistBrowserNavigationStore playlistBrowserNavigationStore)
        {
            _musicService = musicService;

            ResultMedia = new ObservableCollection<YoutubeVideoInfoModel>();

            LoadingWrapper = new ObjectWrapper<bool>();

            FailedWrapper = new ObjectWrapper<bool>();

            SearchMedia = new SearchSongOnYoutubeCommand(youtubeClient, ResultMedia, LoadingWrapper, FailedWrapper);

            

            NavigateHome = new SwitchPageToHomeCommand(navigationService, playlistBrowserNavigationStore);

            DownloadMedia = new DownloadSongOnYoutubeCommand(youtubeClient, ResultMedia);

            _mediaStore = mediaStore;

            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");

            Task.Run(LoadSongs);
        }

        private void LoadSongs()
        {

        }

        public override void Dispose()
        {
            
        }
    }
}
