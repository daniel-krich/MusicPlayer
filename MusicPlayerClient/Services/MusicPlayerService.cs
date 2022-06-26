using Microsoft.EntityFrameworkCore;
using MusicPlayerData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using MusicPlayerData.DataEntities;
using System.IO;
using System.Diagnostics;
using NAudio.Wave.SampleProviders;
using MusicPlayerClient.Events;
using MusicPlayerClient.Stores;

namespace MusicPlayerClient.Services
{
    public interface IMusicPlayerService
    {
        public event EventHandler<MusicPlayerEventArgs>? MusicPlayerEvent;
        public string PlayingSongPath { get; }
        public string PlayingSongName { get; }
        public MediaEntity? CurrentMedia { get; }
        public PlaybackState PlayerState { get; }
        public float Volume { get; set; }
        public double Position { get; set; }
        public double TotalTime { get; }
        public void Play(int mediaId);
        public void Stop();
        public void RePlay();
        public void PlayPause();
        public void PlayNext(bool callStoppedPlay = true);
        public void PlayPrevious();
        public void Skip(TimeSpan time);
    }

    public class MusicPlayerService : IMusicPlayerService
    {
        private readonly MediaStore _mediaStore;
        private IWavePlayer _waveOutDevice;
        private IWaveProvider? _audioFile;
        private MediaEntity? _currentMedia;
        public MediaEntity? CurrentMedia => _currentMedia;

        public event EventHandler<MusicPlayerEventArgs>? MusicPlayerEvent;

        public float Volume
        {
            get => _waveOutDevice?.Volume ?? 0;
            set
            {
                if(_waveOutDevice != null && value >= 0 && value <= 1)
                {
                    _waveOutDevice.Volume = value;
                }
            }
        }

        public double Position
        {
            get => (_audioFile as AudioFileReader)?.CurrentTime.TotalSeconds ?? 0;
            set
            {
                var audio = _audioFile as AudioFileReader;
                if (audio != null)
                {
                    audio.Position = (long?)(audio.WaveFormat.AverageBytesPerSecond * value) ?? 0;
                }
            }
        }

        public double TotalTime
        {
            get
            {
                var audio = _audioFile as AudioFileReader;
                if(audio != null)
                {
                    return audio.Length / audio.WaveFormat.AverageBytesPerSecond;
                }
                return 0;
            }
        }

        public string PlayingSongPath => _currentMedia?.FilePath ?? "";

        public string PlayingSongName => Path.GetFileName(_currentMedia?.FilePath) ?? "";

        public PlaybackState PlayerState => _waveOutDevice?.PlaybackState ?? PlaybackState.Stopped;

        public MusicPlayerService(MediaStore mediaStore)
        {
            _mediaStore = mediaStore;
            _waveOutDevice = new WaveOut();
            _waveOutDevice.PlaybackStopped += OnStoppedPlay;
        }

        public void ChangeVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void Play(int mediaId)
        {
            OnPausePlay();
            _currentMedia = _mediaStore.Songs.FirstOrDefault(x => x.Id == mediaId);
            if (_currentMedia != null)
            {
                _waveOutDevice?.Stop();
                _waveOutDevice?.Dispose();

                try
                {
                    _audioFile = new AudioFileReader(_currentMedia.FilePath);
                    _waveOutDevice = new WaveOut();
                    _waveOutDevice.PlaybackStopped += OnStoppedPlay;
                    _waveOutDevice.Init(_audioFile);
                    _waveOutDevice.Play();
                    OnStartPlay();
                }
                catch
                {
                    Stop();
                }
            }
        }

        public void PlayPause()
        {
            if (_waveOutDevice == null) return;
            if (_waveOutDevice.PlaybackState == PlaybackState.Paused)
            {
                _waveOutDevice.Play();
                OnStartPlay();
            }
            else
            {
                _waveOutDevice.Pause();
                OnPausePlay();
            }
        }

        public void RePlay()
        {
            if (_audioFile != null)
            {
                _waveOutDevice?.Stop();
                _waveOutDevice?.Dispose();

                _waveOutDevice = new WaveOut();
                _waveOutDevice.PlaybackStopped += OnStoppedPlay;
                Position = 0;
                _waveOutDevice.Init(_audioFile);
                _waveOutDevice.Play();
                OnStartPlay();
            }
        }

        public void Stop()
        {
            _currentMedia = null;
            (_audioFile as AudioFileReader)?.Dispose();
            _audioFile = null;
            _waveOutDevice?.Stop();
            _waveOutDevice?.Dispose();

            OnStoppedPlay(this, new StoppedEventArgs());
        }

        public void PlayNext(bool callStoppedPlay = true)
        {
            if (_currentMedia != null)
            {
                var tempmedia = _mediaStore.Songs.FirstOrDefault(x => x.Id > _currentMedia.Id && _currentMedia.PlayerlistId == x.PlayerlistId);
                if (tempmedia != null)
                {
                    _waveOutDevice?.Stop();
                    _waveOutDevice?.Dispose();

                    if(callStoppedPlay)
                        OnStoppedPlay(this, new StoppedEventArgs());

                    _currentMedia = tempmedia;

                    try
                    {
                        _audioFile = new AudioFileReader(_currentMedia.FilePath);
                        _waveOutDevice = new WaveOut();
                        _waveOutDevice.PlaybackStopped += OnStoppedPlay;
                        _waveOutDevice.Init(_audioFile);
                        _waveOutDevice.Play();
                        OnStartPlay();
                    }
                    catch
                    {
                        Stop();
                    }
                }
            }
        }

        public void PlayPrevious()
        {
            if (_currentMedia != null)
            {
                var tempmedia = _mediaStore.Songs.Reverse().FirstOrDefault(x => x.Id < _currentMedia.Id && _currentMedia.PlayerlistId == x.PlayerlistId);
                if (tempmedia != null)
                {
                    _waveOutDevice?.Stop();
                    _waveOutDevice?.Dispose();

                    OnStoppedPlay(this, new StoppedEventArgs());

                    _currentMedia = tempmedia;

                    try
                    {
                        _audioFile = new AudioFileReader(_currentMedia.FilePath);
                        _waveOutDevice = new WaveOut();
                        _waveOutDevice.PlaybackStopped += OnStoppedPlay;
                        _waveOutDevice.Init(_audioFile);
                        _waveOutDevice.Play();
                        OnStartPlay();
                    }
                    catch
                    {
                        Stop();
                    }
                }
            }
        }

        public void Skip(TimeSpan time)
        {
            _waveOutDevice?.Stop();
            _waveOutDevice?.Dispose();

            var skip = _audioFile.ToSampleProvider().Skip(time);

            _waveOutDevice = new WaveOut();
            _waveOutDevice.PlaybackStopped += OnStoppedPlay;
            _waveOutDevice.Init(skip);
            _waveOutDevice.Play();
            OnStartPlay();
        }

        private void OnStoppedPlay(object? sender, StoppedEventArgs e)
        {
            MusicPlayerEvent?.Invoke(this, new MusicPlayerEventArgs(PlayerEventType.Stopped, _currentMedia, _audioFile));
        }

        private void OnStartPlay()
        {
            MusicPlayerEvent?.Invoke(this, new MusicPlayerEventArgs(PlayerEventType.Playing, _currentMedia, _audioFile));
        }

        private void OnPausePlay()
        {
            MusicPlayerEvent?.Invoke(this, new MusicPlayerEventArgs(PlayerEventType.Paused, _currentMedia, _audioFile));
        }
    }
}
