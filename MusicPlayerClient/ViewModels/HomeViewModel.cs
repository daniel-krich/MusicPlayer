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
using MusicPlayerClient.Enums;
using System.Threading;

namespace MusicPlayerClient.ViewModels
{
    public class HomeViewModel : ViewModelBase, IFilesDropAsync
    {
        private readonly IMusicPlayerService _musicService;
        private readonly MediaStore _mediaStore;

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

        private ObservableCollection<MediaModel>? _allSongs;
        public ObservableCollection<MediaModel>? AllSongs
        {
            get => _allSongs;
            set
            {
                _allSongs = value;
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

        public HomeViewModel(IDbContextFactory<DataContext> dbContextFactory, MediaStore mediaStore, IMusicPlayerService musicService)
        {
            _musicService = musicService;
            _mediaStore = mediaStore;
        }

        public override Task InitViewModel()
        {
            _musicService.MusicPlayerEvent += OnMusicPlayerEvent;
            _mediaStore.PlaylistSongsAdded += OnPlaylistSongsAdded;

            PlaySong = new PlaySpecificSongCommand(_musicService);

            OpenExplorer = new OpenExplorerAtPathCommand();

            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");

            Task.Run(LoadSongs);
            return Task.CompletedTask;
        }

        private void LoadSongs()
        {
            AllSongs = new ObservableCollection<MediaModel>(_mediaStore.Songs.Where(x => x.PlayerlistId == null).Select((x, num) =>
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

            DeleteSong = new DeleteSpecificSongAsyncCommand(_musicService, _mediaStore, AllSongs);
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    var songPlay = AllSongs?.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if(songPlay != null)
                    {
                        songPlay.Playing = true;
                    }
                    break;
                default:
                    var songStopped = AllSongs?.FirstOrDefault(x => x.Id == e.Media?.Id);
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
                if (mediaEntity.PlayerlistId is null)
                {
                    var songsIndex = AllSongs?.Count;
                    AllSongs?.Add(new MediaModel
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
                FilePath = x
            }).ToList();

            await _mediaStore.AddRange(mediaEntities);

            foreach (MediaEntity mediaEntity in mediaEntities)
            {
                var songsIndex = AllSongs?.Count;
                AllSongs?.Add(new MediaModel
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
