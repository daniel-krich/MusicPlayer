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
    public class SearchSongOnYoutubeAsyncCommand : AsyncCommandBase
    {
        private readonly IYouTubeClientService _youtubeClient;
        private readonly ObservableCollection<YoutubeVideoInfoModel> _observableMedia;
        private readonly ObservableWrapper<bool>? _loadingWrapper;
        private readonly ObservableWrapper<bool>? _failedWrapper;
        public SearchSongOnYoutubeAsyncCommand(IYouTubeClientService youtubeClient, ObservableCollection<YoutubeVideoInfoModel> observableMedia)
        {
            _youtubeClient = youtubeClient;
            _observableMedia = observableMedia;
        }

        public SearchSongOnYoutubeAsyncCommand(IYouTubeClientService youtubeClient, ObservableCollection<YoutubeVideoInfoModel> observableMedia, ObservableWrapper<bool> loadingWrapper, ObservableWrapper<bool> failedWrapper) : this(youtubeClient, observableMedia)
        {
            _loadingWrapper = loadingWrapper;
            _failedWrapper = failedWrapper;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if(parameter is string searchText)
            {
                _observableMedia.Clear();

                if (_loadingWrapper != null)
                    _loadingWrapper.Object = true;

                List<YoutubeVideoInfo>? videos = await _youtubeClient.SearchVideoByName(searchText);

                if(videos == null)
                {
                    if (_loadingWrapper != null)
                        _loadingWrapper.Object = false;

                    if (_failedWrapper != null)
                        _failedWrapper.Object = true;

                    return;
                }

                

                var videomodels = videos.Select((x, num) => new YoutubeVideoInfoModel
                {
                    Num = num + 1,
                    Title = x.Title,
                    Url = x.Url,
                    Duration = x.Duration,
                    Channel = x.Channel,
                    Views = x.Views
                });

                foreach(var videomodel in videomodels)
                {
                    _observableMedia.Add(videomodel);
                }

                if (_failedWrapper != null)
                    _failedWrapper.Object = false;

                if (_loadingWrapper != null)
                    _loadingWrapper.Object = false;
            }
        }
    }
}
