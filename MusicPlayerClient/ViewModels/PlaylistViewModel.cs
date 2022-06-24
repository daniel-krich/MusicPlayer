using MusicPlayerClient.Commands;
using MusicPlayerClient.Events;
using MusicPlayerClient.Models;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayerClient.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        private readonly IMusicPlayerService _musicService;
        public string CurrentDateString { get; }
        public string CurrentPlaylistName { get; }
        public string PlaylistCreationDate { get; }
        public ObservableCollection<MediaModel> AllSongsOfPlaylist { get; set; }
        public ICommand NavigateHome { get; }
        public ICommand PlaySong { get; }

        public PlaylistViewModel(IMusicPlayerService musicService, INavigationService navigationService, MediaStore mediaStore, PlaylistStore playlistStore, PlaylistBrowserNavigationStore playlistBrowserNavigationStore)
        {
            _musicService = musicService;

            _musicService.MusicPlayerEvent += OnMusicPlayerEvent;

            PlaySong = new PlaySpecificSongCommand(musicService);

            CurrentPlaylistName = playlistStore.Playlists.FirstOrDefault(x => x.Id == playlistBrowserNavigationStore.BrowserPlaylistId)?.Name ?? "Undefined";

            CurrentDateString = DateTime.Now.ToString("dd MMM, yyyy");

            PlaylistCreationDate = playlistStore.Playlists.FirstOrDefault(x => x.Id == playlistBrowserNavigationStore.BrowserPlaylistId)?.CreationDate?.ToString("dd MMM, yyyy") ?? DateTime.Now.ToString("dd MMM, yyyy");

            NavigateHome = new SwitchPageToHomeCommand(navigationService);

            AllSongsOfPlaylist = new ObservableCollection<MediaModel>(mediaStore.Songs.Where(x => x.PlayerlistId == playlistBrowserNavigationStore.BrowserPlaylistId).Select((x, num) =>
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
                    var songPlay = AllSongsOfPlaylist.FirstOrDefault(x => x.Id == e.Media?.Id);
                    if (songPlay != null)
                    {
                        songPlay.CurrentPlayerIconPath = "../icons/pause.svg";
                    }
                    break;
                default:
                    var songStopped = AllSongsOfPlaylist.FirstOrDefault(x => x.Id == e.Media?.Id);
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
