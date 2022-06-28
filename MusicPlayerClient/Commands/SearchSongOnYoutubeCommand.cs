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
    public class SearchSongOnYoutubeCommand : CommandBase
    {
        private readonly IYouTubeClientService _youtubeClient;
        private readonly ObservableCollection<YoutubeVideoInfoModel> _observableMedia;
        private readonly ObjectWrapper<bool>? _loadingWrapper;
        private readonly ObjectWrapper<bool>? _failedWrapper;
        public SearchSongOnYoutubeCommand(IYouTubeClientService youtubeClient, ObservableCollection<YoutubeVideoInfoModel> observableMedia)
        {
            _youtubeClient = youtubeClient;
            _observableMedia = observableMedia;
        }

        public SearchSongOnYoutubeCommand(IYouTubeClientService youtubeClient, ObservableCollection<YoutubeVideoInfoModel> observableMedia, ObjectWrapper<bool> loadingWrapper, ObjectWrapper<bool> failedWrapper) : this(youtubeClient, observableMedia)
        {
            _loadingWrapper = loadingWrapper;
            _failedWrapper = failedWrapper;
        }

        public override async void Execute(object? parameter)
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
