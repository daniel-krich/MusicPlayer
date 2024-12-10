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
using MusicPlayerData.DataEntities;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicPlayerClient.ViewModels
{
    public class PlaylistViewModel : ViewModelBase, IFilesDropAsync
    {
        private readonly IMusicPlayerService _musicService;
        private readonly PlaylistBrowserNavigationDispatcher _playlistBrowserNavigationDispacher;
        private readonly MediaStore _mediaStore;
        private readonly PlaylistStore _playlistStore;

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

        private string? _currentPlaylistName;
        public string? CurrentPlaylistName
        {
            get => _currentPlaylistName;
            set
            {
                _currentPlaylistName = value;
                OnPropertyChanged();
            }
        }

        private string? _playlistCreationDate;
        public string? PlaylistCreationDate
        {
            get => _playlistCreationDate;
            set
            {
                _playlistCreationDate = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MediaModel>? _allSongsOfPlaylist;
        public ObservableCollection<MediaModel>? AllSongsOfPlaylist
        {
            get => _allSongsOfPlaylist;
            set
            {
                _allSongsOfPlaylist = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _deleteSong;
        public ICommand? DeleteSong
        {
            get => _deleteSong;
            set
            {
                _deleteSong = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _openExplorer;
        public ICommand? OpenExplorer
        {
            get => _openExplorer;
            set
            {
                _openExplorer = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _playSong;
        public ICommand? PlaySong
        {
            get => _playSong;
            set
            {
                _playSong = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _renamePlaylist;
        public ICommand? RenamePlaylist
        {
            get => _renamePlaylist;
            set
            {
                _renamePlaylist = value;
                OnPropertyChanged();
            }
        }

        public PlaylistViewModel(IMusicPlayerService musicService, INavigationService navigationService, MediaStore mediaStore, PlaylistStore playlistStore, PlaylistBrowserNavigationDispatcher playlistBrowserNavigationDispacher)
        {
            _musicService = musicService;
            _playlistBrowserNavigationDispacher = playlistBrowserNavigationDispacher;
            _mediaStore = mediaStore;
            _playlistStore = playlistStore;
        }

        public override Task InitViewModel()
        {
            RenamePlaylist = new RenamePlaylistAsyncCommand(_playlistStore, _playlistBrowserNavigationDispacher);

            _musicService.MusicPlayerEvent += OnMusicPlayerEvent;
            _mediaStore.PlaylistSongsAdded += OnPlaylistSongsAdded;

            PlaySong = new PlaySpecificSongCommand(_musicService);

            OpenExplorer = new OpenExplorerAtPathCommand();

            CurrentPlaylistName = _playlistStore.Playlists.FirstOrDefault(x => x.Id == _playlistBrowserNavigationDispacher.BrowserPlaylistId)?.Name ?? "Undefined";

            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");

            PlaylistCreationDate = _playlistStore.Playlists.FirstOrDefault(x => x.Id == _playlistBrowserNavigationDispacher.BrowserPlaylistId)?.CreationDate?.ToString("dd MMM, yyyy") ?? DateTime.Now.ToString("dd MMM, yyyy");

            Task.Run(LoadSongs);
            return Task.CompletedTask;
        }

        private void LoadSongs()
        {
            AllSongsOfPlaylist = new ObservableCollection<MediaModel>(_mediaStore.Songs.Where(x => x.PlayerlistId == _playlistBrowserNavigationDispacher.BrowserPlaylistId).Select((x, num) =>
            {
                return new MediaModel
                {
                    Playing = _musicService.PlayerState == PlaybackState.Playing && x.Id == _musicService.CurrentMedia?.Id,
                    Number = num + 1,
                    Id = x.Id,
                    Title = Path.GetFileNameWithoutExtension(x.FilePath),
                    Path = x.FilePath,
                    Duration = AudioUtills.DurationParse(x.FilePath)
                };
            }).ToList());

            DeleteSong = new DeleteSpecificSongAsyncCommand(_musicService, _mediaStore, AllSongsOfPlaylist);
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    var songPlay = AllSongsOfPlaylist?.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if (songPlay != null)
                    {
                        songPlay.Playing = true;
                    }
                    break;
                default:
                    var songStopped = AllSongsOfPlaylist?.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if (songStopped != null)
                    {
                        songStopped.Playing = false;
                    }
                    break;
            }
        }

        private void OnPlaylistSongsAdded(object? sender, PlaylistSongsAddedEventArgs e)
        {
            foreach (MediaEntity mediaEntity in e.Songs)
            {
                if (mediaEntity.PlayerlistId == _playlistBrowserNavigationDispacher.BrowserPlaylistId)
                {
                    var songsIndex = AllSongsOfPlaylist?.Count;
                    AllSongsOfPlaylist?.Add(new MediaModel
                    {
                        Playing = _musicService.PlayerState == PlaybackState.Playing && mediaEntity.Id == _musicService.CurrentMedia?.Id,
                        Number = songsIndex + 1,
                        Id = mediaEntity.Id,
                        Title = Path.GetFileNameWithoutExtension(mediaEntity.FilePath),
                        Path = mediaEntity.FilePath,
                        Duration = AudioUtills.DurationParse(mediaEntity.FilePath)
                    });
                }
            }
        }

        public async Task OnFilesDroppedAsync(string[] files, object? parameter)
        {
            var mediaEntities = files.Where(x => PathExtension.HasAudioVideoExtensions(x)).Select(x => new MediaEntity
            {
                PlayerlistId = _playlistBrowserNavigationDispacher.BrowserPlaylistId,
                FilePath = x
            }).ToList();

            await _mediaStore.AddRange(mediaEntities);

            foreach (MediaEntity mediaEntity in mediaEntities)
            {
                var songsIndex = AllSongsOfPlaylist?.Count;
                AllSongsOfPlaylist?.Add(new MediaModel
                {
                    Playing = _musicService.PlayerState == PlaybackState.Playing && mediaEntity.Id == _musicService.CurrentMedia?.Id,
                    Number = songsIndex + 1,
                    Id = mediaEntity.Id,
                    Title = Path.GetFileNameWithoutExtension(mediaEntity.FilePath),
                    Path = mediaEntity.FilePath,
                    Duration = AudioUtills.DurationParse(mediaEntity.FilePath)
                });
            }
        }

        public override void Dispose()
        {
            _musicService.MusicPlayerEvent -= OnMusicPlayerEvent;
            _mediaStore.PlaylistSongsAdded -= OnPlaylistSongsAdded;
        }
    }
}
