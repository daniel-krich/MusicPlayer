﻿using System.Net;
using System.Linq;
using System.Text.Json;
using VideoLibrary;
using Newtonsoft.Json.Linq;
using MusicDownloader.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Web;
using System.Net.Http.Headers;

namespace MusicPlayerClient.Services
{
    public interface IYouTubeClientService
    {
        IAsyncEnumerable<int> DownloadYoutubeAudioAsync(string url, string FileName);
        Task<List<YoutubeVideoInfo>?> SearchVideoByName(string query);
        string GetSafeFileName(string name, char replace = ' ');
    }

    public class YouTubeClientService : IYouTubeClientService
    {
        public const string YouTubeSearchUrl = "https://www.youtube.com/results?search_query=";
        public const string YouTubeBase = "https://www.youtube.com";
        public async Task<List<YoutubeVideoInfo>?> SearchVideoByName(string query)
        {
            List<YoutubeVideoInfo> videos = new List<YoutubeVideoInfo>();
            try
            {
                using (var client = new HttpClient())
                {
                    var queryEncoded = query.Contains("//:") ? query : HttpUtility.UrlEncode(query);
                    string ? response = await client.GetStringAsync(YouTubeSearchUrl + queryEncoded);
                    if (response != null)
                    {
                        string start = "var ytInitialData = ";
                        string end = "};";
                        var startIndex = response.IndexOf(start) + start.Length;
                        var endIndex = response.IndexOf(end, startIndex);

                        var InitialData = JObject.Parse(response.Substring(startIndex, endIndex + 1 - startIndex));

                        var results = InitialData?["contents"]?["twoColumnSearchResultsRenderer"]?["primaryContents"]?["sectionListRenderer"]?["contents"]?[0]?["itemSectionRenderer"]?["contents"];

                        if (results != null)
                        {
                            foreach (var item in results)
                            {
                                var video_info = item["videoRenderer"];
                                var title = video_info?["title"]?["runs"]?[0]?["text"];
                                var url = video_info?["navigationEndpoint"]?["commandMetadata"]?["webCommandMetadata"]?["url"];
                                var length = video_info?["lengthText"]?["simpleText"];
                                var views = video_info?["shortViewCountText"]?["simpleText"];
                                var channel = video_info?["ownerText"]?["runs"]?[0]?["text"];

                                if (title != null && url != null && length != null
                                    && channel != null && views != null)
                                {
                                    videos.Add(new YoutubeVideoInfo
                                    {
                                        Title = GetSafeFileName(title.ToString()),
                                        Url = YouTubeBase + url.ToString(),
                                        Duration = length.ToString(),
                                        Channel = channel.ToString(),
                                        Views = views.ToString()
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return default;
            }

            return videos;
        }

        public async IAsyncEnumerable<int> DownloadYoutubeAudioAsync(string url, string FileName)
        {
            var youTube = YouTube.Default;
            var videos = await youTube.GetAllVideosAsync(url);
            var video = videos.First(x => x.AdaptiveKind == AdaptiveKind.Audio);

            long contentLength = 0;

            using (var client = new HttpClient())
            {
                var res = await (await Task.Factory.StartNew(() => client.SendAsync(new HttpRequestMessage(HttpMethod.Head, video.Uri))));
                contentLength = res.Content.Headers.ContentLength ?? 0;
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Range = new RangeHeaderValue(0, null);

                using var videoStream = await client.GetStreamAsync(video.Uri);
                using var file = File.Create(FileName);

                byte[] buffer = new byte[8192];

                long numBytesRead = 0;
                int currentBytes = 0;

                while (numBytesRead < contentLength)
                {
                    currentBytes = await videoStream.ReadAsync(buffer, 0, buffer.Length);
                    numBytesRead += currentBytes;
                    await file.WriteAsync(buffer, 0, currentBytes);

                    double percent = numBytesRead / (contentLength * 1.0);
                    yield return (int)(percent * 100);
                }
            }
        }

        public string GetSafeFileName(string name, char replace = ' ')
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            return new string(name.Select(c => invalids.Contains(c) ? replace : c).ToArray());
        }
    }
}