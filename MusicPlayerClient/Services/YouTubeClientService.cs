using System.Net;
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

namespace MusicPlayerClient.Services
{
    public interface IYouTubeClientService
    {
        IAsyncEnumerable<int> DownloadYoutubeAudioAsync(string url, string FileName);
        Task<List<YoutubeVideoInfo>?> SearchVideoByName(string query);
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
                    string? response = await client.GetStringAsync(YouTubeSearchUrl + query);
                    if (response != null)
                    {
                        string start = "var ytInitialData = ";
                        string end = ";";
                        var startIndex = response.IndexOf(start) + start.Length;
                        var endIndex = response.IndexOf(end, startIndex);

                        var InitialData = JObject.Parse(response.Substring(startIndex, endIndex - startIndex));
                        var results = InitialData?["contents"]?["twoColumnSearchResultsRenderer"]?["primaryContents"]?["sectionListRenderer"]?["contents"]?[0]?["itemSectionRenderer"]?["contents"];

                        if (results != null)
                        {
                            foreach (var item in results)
                            {
                                var video_info = item["videoRenderer"];
                                var title = video_info?["title"]?["runs"]?[0]?["text"];
                                var url = video_info?["navigationEndpoint"]?["commandMetadata"]?["webCommandMetadata"]?["url"];
                                var length = video_info?["lengthText"]?["simpleText"];

                                if (title != null && url != null && length != null)
                                {
                                    videos.Add(new YoutubeVideoInfo
                                    {
                                        Title = GetSafeFileName(title.ToString()),
                                        Url = YouTubeBase + url?.ToString(),
                                        Duration = length.ToString()
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
            var video = await youTube.GetVideoAsync(url);

            long contentLength = 0;

            using (var client = new HttpClient())
            {
                var res = await (await Task.Factory.StartNew(() => client.SendAsync(new HttpRequestMessage(HttpMethod.Head, video.Uri))));
                contentLength = res.Content.Headers.ContentLength ?? 0;
            }

            using var videoStream = await video.StreamAsync();
            using var file = File.Create(FileName);

            byte[] buffer = new byte[4096*50];

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

        private string GetSafeFileName(string name, char replace = ' ')
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            return new string(name.Select(c => invalids.Contains(c) ? replace : c).ToArray());
        }
    }
}