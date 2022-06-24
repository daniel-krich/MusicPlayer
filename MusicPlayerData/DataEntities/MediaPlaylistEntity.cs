using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerData.DataEntities
{
    public class MediaPlaylistEntity : BaseEntity
    {
        public int MediaId { get; set; }
        public virtual MediaEntity? Media { get; set; }
        public int PlayerlistId { get; set; }
        public virtual PlaylistEntity? Playerlist { get; set; }
    }
}
