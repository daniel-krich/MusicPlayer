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

namespace MusicPlayerClient.Services
{
    public interface IMusicPlayerService
    {
        public string PlayingSongPath { get; }
        public string PlayingSongName { get; }
        public PlaybackState PlayerState { get; }
        public float Volume { get; set; }
        public double Position { get; set; }
        public double TotalTime { get; }
        public void Play(int mediaId);
        public void RePlay();
        public void PlayPause();
        public void PlayNext();
        public void PlayPrevious();
        public void Skip(TimeSpan time);
    }

    public class MusicPlayerService : IMusicPlayerService
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;
        private IWavePlayer _waveOutDevice;
        private IWaveProvider? _audioFile;
        private MediaEntity? _currentMedia;

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

        public string PlayingSongPath => _currentMedia?.FilePath ?? "None";

        public string PlayingSongName => Path.GetFileName(_currentMedia?.FilePath) ?? "None";

        public PlaybackState PlayerState => _waveOutDevice?.PlaybackState ?? PlaybackState.Stopped;

        public MusicPlayerService(IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _waveOutDevice = new WaveOut();
        }

        public void ChangeVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void Play(int mediaId)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                _currentMedia = dbContext.Songs.Find(mediaId);
                if (_currentMedia != null)
                {
                    _audioFile = new AudioFileReader(_currentMedia.FilePath);
                    _waveOutDevice = new WaveOut();
                    _waveOutDevice.Init(_audioFile);
                    _waveOutDevice.Play();
                }
            }
        }

        public void PlayPause()
        {
            if (_waveOutDevice == null) return;
            if (_waveOutDevice.PlaybackState == PlaybackState.Paused)
            {
                _waveOutDevice.Play();
            }
            else
            {
                _waveOutDevice.Pause();
            }
        }

        public void RePlay()
        {
            if (_audioFile != null)
            {
                _waveOutDevice?.Stop();
                _waveOutDevice?.Dispose();

                _waveOutDevice = new WaveOut();
                Position = 0;
                _waveOutDevice.Init(_audioFile);
                _waveOutDevice.Play();
            }
        }

        public void PlayNext()
        {
            throw new NotImplementedException();
        }

        public void PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public void Skip(TimeSpan time)
        {
            _waveOutDevice?.Stop();
            _waveOutDevice?.Dispose();

            var skip = _audioFile.ToSampleProvider().Skip(time);

            _waveOutDevice = new WaveOut();
            _waveOutDevice.Init(skip);
            _waveOutDevice.Play();
        }
    }
}
