using MusicDownloader;
using System.Text.Json;

YouTubeClientService youtube = new YouTubeClientService();

var videos = await youtube.SearchVideoByName("Calvin");

foreach(var video in videos)
{
    await youtube.DownloadYoutubeAudioAsync(video.Url, $@"./{video.Title}.mp3");
}
