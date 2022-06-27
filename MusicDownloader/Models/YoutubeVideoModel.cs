using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDownloader.Models
{
    public class YoutubeVideoModel
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string Duration { get; set; } = "0:00";
    }
}
