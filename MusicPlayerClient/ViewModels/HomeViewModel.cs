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

namespace MusicPlayerClient.ViewModels
{
    public class HomeViewModel : ViewModelBase, IFilesDrop
    {
        private readonly IMusicPlayerService _musicService;
        private readonly MediaStore _mediaStore;
        public string CurrentDateString { get; }
        public ObservableCollection<MediaModel>? AllSongs { get; set; }
        public string CurrentPlayerIconPath => _musicService.PlayerState == PlaybackState.Playing ? "../icons/pause.svg" : "../icons/play.svg";
        public ICommand PlaySong { get; } 
        public ICommand? DeleteSong { get; set; }

        public HomeViewModel(IDbContextFactory<DataContext> dbContextFactory, MediaStore mediaStore, IMusicPlayerService musicService)
        {
            _musicService = musicService;

            _mediaStore = mediaStore;

            _musicService.MusicPlayerEvent += OnMusicPlayerEvent;

            PlaySong = new PlaySpecificSongCommand(musicService);

            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");

            Task.Run(LoadSongs);
        }

        private void LoadSongs()
        {
            AllSongs = new ObservableCollection<MediaModel>(_mediaStore.Songs.Where(x => x.PlayerlistId == null).Select((x, num) =>
            {
                return new MediaModel
                {
                    CurrentPlayerIconPath = _musicService.PlayerState == PlaybackState.Playing && x.Id == _musicService.CurrentMedia?.Id ? "../icons/pause.svg" : "../icons/play.svg",
                    Number = num + 1,
                    Id = x.Id,
                    Title = Path.GetFileName(x.FilePath),
                    Path = x.FilePath,
                    Duration = AudioFileUtills.DurationParse(x.FilePath)
                };
            }).ToList());

            OnPropertyChanged(nameof(AllSongs));

            DeleteSong = new DeleteSpecificSongCommand(_musicService, _mediaStore, AllSongs);
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    var songPlay = AllSongs?.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if(songPlay != null)
                    {
                        songPlay.CurrentPlayerIconPath = "../icons/pause.svg";
                    }
                    break;
                default:
                    var songStopped = AllSongs?.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if (songStopped != null)
                    {
                        songStopped.CurrentPlayerIconPath = "../icons/play.svg";
                    }
                    break;
            }
        }

        public void OnFilesDropped(string[] files)
        {
            var mediaEntities = files.Where(x => PathExtension.HasOneOfExtensions(x, ".wav", ".mp3")).Select(x => new MediaEntity
            {
                FilePath = x
            }).ToList();

            _mediaStore.AddRange(mediaEntities);

            foreach (MediaEntity mediaEntity in mediaEntities)
            {
                var songsIndex = AllSongs?.Count;
                AllSongs?.Add(new MediaModel
                {
                    CurrentPlayerIconPath = _musicService.PlayerState == PlaybackState.Playing && mediaEntity.Id == _musicService.CurrentMedia?.Id ? "../icons/pause.svg" : "../icons/play.svg",
                    Number = songsIndex + 1,
                    Id = mediaEntity.Id,
                    Title = Path.GetFileName(mediaEntity.FilePath),
                    Path = mediaEntity.FilePath,
                    Duration = AudioFileUtills.DurationParse(mediaEntity.FilePath)
                });
            }
        }

        public override void Dispose()
        {
            _musicService.MusicPlayerEvent -= OnMusicPlayerEvent;
        }
    }
}
