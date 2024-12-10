using Microsoft.EntityFrameworkCore;
using MusicPlayerClient.Commands;
using MusicPlayerClient.Core;
using MusicPlayerClient.Dispachers;
using MusicPlayerClient.Enums;
using MusicPlayerClient.Events;
using MusicPlayerClient.Extensions;
using MusicPlayerClient.Interfaces;
using MusicPlayerClient.Models;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using MusicPlayerData.Data;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MusicPlayerClient.ViewModels
{
    public class ToolbarViewModel : ViewModelBase, IFilesDropAsync
    {
        private readonly PlaylistStore _playlistStore;
        private readonly MediaStore _mediaStore;
        private readonly PlaylistBrowserNavigationDispatcher _playlistBrowserNavigationDispacher;
        private readonly IMusicPlayerService _musicPlayerService;
        private readonly INavigationService _navigationService;

        private PageType _currentPage;
        public PageType CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private bool _isRemoveActive;
        public bool IsRemoveActive
        {
            get => _isRemoveActive;
            set
            {
                _isRemoveActive = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PlaylistModel>? _playlists;
        public ObservableCollection<PlaylistModel>? Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _toggleRemoveActive;
        public ICommand? ToggleRemoveActive
        {
            get => _toggleRemoveActive;
            set
            {
                _toggleRemoveActive = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _deletePlaylist;
        public ICommand? DeletePlaylist
        {
            get => _deletePlaylist;
            set
            {
                _deletePlaylist = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _navigatePlaylist;
        public ICommand? NavigatePlaylist
        {
            get => _navigatePlaylist;
            set
            {
                _navigatePlaylist = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _navigateDownloads;
        public ICommand? NavigateDownloads
        {
            get => _navigateDownloads;
            set
            {
                _navigateDownloads = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _navigateHome;
        public ICommand? NavigateHome
        {
            get => _navigateHome;
            set
            {
                _navigateHome = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _createPlaylist;
        public ICommand? CreatePlaylist
        {
            get => _createPlaylist;
            set
            {
                _createPlaylist = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _togglePlayer;
        public ICommand? TogglePlayer
        {
            get => _togglePlayer;
            set
            {
                _togglePlayer = value;
                OnPropertyChanged();
            }
        }

        public ToolbarViewModel(IMusicPlayerService musicPlayerService, INavigationService navigationService, PlaylistBrowserNavigationDispatcher playlistBrowserNavigationDispacher, PlaylistStore playlistStore, MediaStore mediaStore)
        {
            _playlistStore = playlistStore;
            _mediaStore = mediaStore;
            _musicPlayerService = musicPlayerService;
            _navigationService = navigationService;
            _playlistBrowserNavigationDispacher = playlistBrowserNavigationDispacher;
        }

        public override Task InitViewModel()
        {

            TogglePlayer = new ToggleMusicPlayerStateCommand(_musicPlayerService);

            _playlistStore.PlaylistNameChanged += OnPlaylistNameChanged;

            _musicPlayerService.MusicPlayerEvent += OnMusicPlayerEvent;

            CurrentPage = _navigationService.CurrentPage;
            _navigationService.PageChangedEvent += OnPageChangedEvent;

            _playlistBrowserNavigationDispacher.PlaylistBrowserChanged += OnPlaylistBrowserChanged;

            Playlists = new ObservableCollection<PlaylistModel>(_playlistStore.Playlists.Select(x => new PlaylistModel
            {
                Id = x.Id,
                Name = x.Name,
                CreationDate = x.CreationDate
            }).Reverse().ToList());

            ToggleRemoveActive = new TogglePlaylistRemoveCommand(this);
            NavigateHome = new SwitchPageToHomeAsyncCommand(_navigationService, _playlistBrowserNavigationDispacher);
            NavigatePlaylist = new SwitchPageToPlaylistAsyncCommand(_navigationService, _playlistBrowserNavigationDispacher);
            NavigateDownloads = new SwitchPageToDownloadsAsyncCommand(_navigationService, _playlistBrowserNavigationDispacher);
            DeletePlaylist = new DeleteSpecificPlaylistAsyncCommand(_musicPlayerService, _navigationService, _playlistBrowserNavigationDispacher, _playlistStore, _mediaStore, Playlists);
            CreatePlaylist = new CreatePlaylistAsyncCommand(_playlistStore, Playlists);
            return Task.CompletedTask;
        }

        private void OnPlaylistNameChanged(object? sender, PlaylistNameChangedEventArgs args)
        {
            var playlist = Playlists?.FirstOrDefault(x => x.Id == args.Id);
            if (playlist != null)
            {
                playlist.Name = args.Name;
            }
        }

        private void OnPlaylistBrowserChanged(object? sender, PlaylistBrowserChangedEventArgs args)
        {
            Playlists?.ToList().ForEach(x =>
            {
                if (x.Id == args.PlaylistId)
                {
                    x.IsSelected = true;
                }
                else
                {
                    x.IsSelected = false;
                }
            });
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    Playlists?.ToList().ForEach(x =>
                    {
                        if (x.Id == e.Media?.PlayerlistId)
                        {
                            x.IsPlaying = true;
                        }
                        else
                        {
                            x.IsPlaying = false;
                        }
                    });
                    break;
                default:
                    Playlists?.ToList().ForEach(x => x.IsPlaying = false);
                    break;
            }
        }

        private void OnPageChangedEvent(object? sender, PageChangedEventArgs args)
        {
            CurrentPage = args.Page;
        }

        public async Task OnFilesDroppedAsync(string[] files, object? parameter)
        {
            if (parameter is int playlistId)
            {
                var mediaEntities = files.Where(x => PathExtension.HasAudioVideoExtensions(x)).Select(x => new MediaEntity
                {
                    FilePath = x,
                    PlayerlistId = playlistId
                }).ToList();

                await _mediaStore.AddRange(mediaEntities, true);
            }
            else // Add to main playlist
            {
                var mediaEntities = files.Where(x => PathExtension.HasAudioVideoExtensions(x)).Select(x => new MediaEntity
                {
                    FilePath = x
                }).ToList();

                await _mediaStore.AddRange(mediaEntities, true);
            }
        }

        public override void Dispose()
        {
            _playlistStore.PlaylistNameChanged -= OnPlaylistNameChanged;
            _playlistBrowserNavigationDispacher.PlaylistBrowserChanged -= OnPlaylistBrowserChanged;
            _musicPlayerService.MusicPlayerEvent -= OnMusicPlayerEvent;
            _navigationService.PageChangedEvent -= OnPageChangedEvent;
        }
    }
}
