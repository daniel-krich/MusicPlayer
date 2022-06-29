using MusicPlayerClient.Services;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayerClient.Commands;
using NAudio.Wave;
using MusicPlayerClient.Events;
using MusicPlayerClient.Core;
using MusicPlayerClient.Extensions;

namespace MusicPlayerClient.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly IMusicPlayerService _musicService;

        public int Volume
        {
            get => (int)Math.Ceiling(_musicService.Volume * 100);
            set
            {
                _musicService.Volume = value / 100f;
                OnPropertyChanged();
            }
        }

        private string? _songName;
        public string? SongName
        {
            get => _songName;
            set
            {
                _songName = value;
                OnPropertyChanged();
            }
        }

        private string? _songPath;
        public string? SongPath
        {
            get => _songPath;
            set
            {
                _songPath = value;
                OnPropertyChanged();
            }
        }

        public double SongProgress
        {
            get => _musicService.Position;
            set
            {
                _musicService.Position = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SongProgressFormatted));
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public string SongProgressFormatted => AudioUtills.DurationParse(SongProgress);

        public double SongDuration => _musicService.TotalTime;

        public string SongDurationFormatted => AudioUtills.DurationParse(SongDuration);

        public ICommand TogglePlayer { get; }
        public ICommand PlayBackward { get; }
        public ICommand PlayForward { get; }

        public PlayerViewModel(IMusicPlayerService musicService)
        {
            _musicService = musicService;
            PlayBackward = new BackwardSongCommand(musicService);
            PlayForward = new ForwardSongCommand(musicService);
            TogglePlayer = new ToggleMusicPlayerStateCommand(musicService);

            _musicService.MusicPlayerEvent += OnMusicPlayerEvent;

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SongProgress));
            OnPropertyChanged(nameof(SongDuration));
            OnPropertyChanged(nameof(SongProgressFormatted));
            OnPropertyChanged(nameof(SongDurationFormatted));
        }

        private void OnMusicPlayerEvent(object? sender, MusicPlayerEventArgs e)
        {
            switch (e.Type)
            {
                case PlayerEventType.Playing:
                    IsPlaying = true;
                    break;
                case PlayerEventType.Finished:
                    IsPlaying = false;
                    _musicService.PlayNext(false);
                    break;
                default:
                    IsPlaying = false;
                    break;
            }

            SongName = _musicService.PlayingSongName;
            SongPath = _musicService.PlayingSongPath;
        }

        public override void Dispose()
        {
            _musicService.MusicPlayerEvent -= OnMusicPlayerEvent;
        }
    }
}
