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

namespace MusicPlayerClient.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IMusicPlayerService _musicService;
        public string CurrentDateString { get; }
        public ObservableCollection<MediaModel> AllSongs { get; set; }
        public string CurrentPlayerIconPath => _musicService.PlayerState == PlaybackState.Playing ? "../icons/pause.svg" : "../icons/play.svg";
        public ICommand PlaySong { get; }  

        public HomeViewModel(IDbContextFactory<DataContext> dbContextFactory, MediaStore mediaStore, IMusicPlayerService musicService)
        {
            _musicService = musicService;

            _musicService.MusicPlayerEvent += OnMusicPlayerEvent;

            PlaySong = new PlaySpecificSongCommand(musicService);

            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");

            AllSongs = new ObservableCollection<MediaModel>(mediaStore.Songs.Where(x => x.PlayerlistId == null).Select((x, num) =>
            {
                using (var audioFile = new AudioFileReader(x.FilePath))
                {
                    var durationParse = $"{Math.Floor(audioFile.TotalTime.TotalSeconds / 60).ToString().PadLeft(2, '0')}:{Math.Floor(audioFile.TotalTime.TotalSeconds % 60).ToString().PadLeft(2, '0')}";
                    return new MediaModel
                    {
                        CurrentPlayerIconPath = _musicService.PlayerState == PlaybackState.Playing && x.Id == _musicService.CurrentMedia?.Id ? "../icons/pause.svg" : "../icons/play.svg",
                        Number = num + 1,
                        Id = x.Id,
                        Title = Path.GetFileName(x.FilePath),
                        Path = x.FilePath,
                        Duration = durationParse
                    };
                }
            }).ToList());
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    var songPlay = AllSongs.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if(songPlay != null)
                    {
                        songPlay.CurrentPlayerIconPath = "../icons/pause.svg";
                    }
                    break;
                default:
                    var songStopped = AllSongs.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if (songStopped != null)
                    {
                        songStopped.CurrentPlayerIconPath = "../icons/play.svg";
                    }
                    break;
            }
        }

        public override void Dispose()
        {
            _musicService.MusicPlayerEvent -= OnMusicPlayerEvent;
        }
    }
}
