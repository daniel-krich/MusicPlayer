﻿using MusicPlayerClient.Extensions;
using MusicPlayerClient.Models;
using MusicPlayerClient.Services;
using MusicPlayerClient.Stores;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class DeleteSpecificSongAsyncCommand : AsyncCommandBase
    {
        private readonly IMusicPlayerService _musicService;
        private readonly MediaStore _mediaStore;
        private readonly ObservableCollection<MediaModel>? _observableSongs;
        public DeleteSpecificSongAsyncCommand(IMusicPlayerService musicService, MediaStore mediaStore)
        {
            _musicService = musicService;
            _mediaStore = mediaStore;
        }

        public DeleteSpecificSongAsyncCommand(IMusicPlayerService musicService, MediaStore mediaStore, ObservableCollection<MediaModel> observableSongs) : this(musicService, mediaStore)
        {
            _observableSongs = observableSongs;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if (parameter is int SongId)
            {
                if (_musicService.CurrentMedia?.Id == SongId)
                {
                    _musicService.Stop();
                }

                _observableSongs?.RemoveAll(x => x.Id == SongId);

                await _mediaStore.Remove(SongId);
            }
        }
    }
}
