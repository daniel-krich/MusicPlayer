using MusicDownloader.Models;
using MusicPlayerClient.Core;
using MusicPlayerClient.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayerClient.Commands
{
    public class DownloadSongOnYoutubeCommand : CommandBase
    {
        private readonly IYouTubeClientService _youtubeClient;
        private readonly ObservableCollection<YoutubeVideoInfoModel> _observableMedia;

        public DownloadSongOnYoutubeCommand(IYouTubeClientService youtubeClient, ObservableCollection<YoutubeVideoInfoModel> observableMedia)
        {
            _youtubeClient = youtubeClient;
            _observableMedia = observableMedia;
        }

        public override async void Execute(object? parameter)
        {
            if(parameter is string url)
            {
                YoutubeVideoInfoModel? video = _observableMedia.FirstOrDefault(x => x.Url == url);
                if (video != null && !video.Downloading)
                {
                    video.Downloading = true;
                    var download = _youtubeClient.DownloadYoutubeAudioAsync(video.Url!, @"downloads/" + video.Title + ".mp3");
                    await foreach(var progress in download)
                    {
                        video.DownloadProgress = progress;
                    }
                }
            }
        }
    }
}
