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
using System.Windows.Controls;
using MusicPlayerClient.Dispachers;

namespace MusicPlayerClient.ViewModels
{
    public class DownloadsViewModel : ViewModelBase
    {
        private readonly IMusicPlayerService _musicService;
        private readonly MediaStore _mediaStore;
        private readonly IYouTubeClientService _youtubeClient;

        private bool _isLoadingSearch;
        public bool IsLoadingSearch
        {
            get => _isLoadingSearch;
            set
            {
                _isLoadingSearch = value;
                OnPropertyChanged();
            }
        }

        private string? _currentDateString;
        public string? CurrentDateString
        {
            get => _currentDateString;
            set
            {
                _currentDateString = value;
                OnPropertyChanged();
            }
        }

        private bool _isFailedSearch;
        public bool IsFailedSearch
        {
            get => _isFailedSearch;
            set
            {
                _isFailedSearch = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<YoutubeVideoInfoModel>? _resultMedia;
        public ObservableCollection<YoutubeVideoInfoModel>? ResultMedia
        {
            get => _resultMedia;
            set
            {
                _resultMedia = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _searchMedia;
        public ICommand? SearchMedia
        {
            get => _searchMedia;
            set
            {
                _searchMedia = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _downloadMedia;
        public ICommand? DownloadMedia
        {
            get => _downloadMedia;
            set
            {
                _downloadMedia = value;
                OnPropertyChanged();
            }
        }

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
        
        public DownloadsViewModel(MediaStore mediaStore, IMusicPlayerService musicService, IYouTubeClientService youtubeClient)
        {
            _musicService = musicService;
            _mediaStore = mediaStore;
            _youtubeClient = youtubeClient;
        }

        public override Task InitViewModel()
        {
            ResultMedia = new ObservableCollection<YoutubeVideoInfoModel>();
            SearchMedia = new SearchSongOnYoutubeAsyncCommand(_youtubeClient, ResultMedia, this);
            DownloadMedia = new DownloadSongOnYoutubeAsyncCommand(_youtubeClient, ResultMedia);
            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            
        }
    }
}
