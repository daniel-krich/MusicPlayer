﻿using MusicPlayerClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDownloader.Models
{
    public class YoutubeVideoInfo
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Duration { get; set; }
    }

    public class YoutubeVideoInfoModel : ObservableObject
    {
        private int _num;
        private bool _downloading;
        private int _downloadProgress;
        private string? _title;
        private string? _url;
        private string? _duration;

        public int Num
        {
            get { return _num; }
            set
            {
                if (_num != value)
                {
                    _num = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Downloading
        {
            get { return _downloading; }
            set
            {
                if (_downloading != value)
                {
                    _downloading = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set
            {
                if (_downloadProgress != value)
                {
                    _downloadProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Url
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
