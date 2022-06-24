﻿using MusicPlayerClient.Services;
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

        public string PlayingSongPath => _musicService.PlayingSongPath;

        public string PlayingSongName => _musicService.PlayingSongName;

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

        public string CurrentPlayerIconPath => _musicService.PlayerState == PlaybackState.Playing ? "../icons/pause.svg" : "../icons/play.svg";

        public string SongProgressFormatted => $"{Math.Floor(SongProgress / 60).ToString().PadLeft(2, '0')}:{Math.Floor(SongProgress % 60).ToString().PadLeft(2, '0')}";

        public double SongDuration => _musicService.TotalTime;

        public string SongDurationFormatted => $"{Math.Floor(SongDuration / 60).ToString().PadLeft(2, '0')}:{Math.Floor(SongDuration % 60).ToString().PadLeft(2, '0')}";

        public ICommand TogglePlayer { get; }

        public PlayerViewModel(IMusicPlayerService musicService)
        {
            _musicService = musicService;
            TogglePlayer = new ToggleMusicPlayerStateCommand(musicService, () => OnPropertyChanged(nameof(CurrentPlayerIconPath)));

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
            OnPropertyChanged(nameof(PlayingSongName));
            OnPropertyChanged(nameof(PlayingSongPath));
            OnPropertyChanged(nameof(CurrentPlayerIconPath));
        }
    }
}
