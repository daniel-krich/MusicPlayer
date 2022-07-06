using MusicPlayerClient.Enums;
using MusicPlayerData.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Events
{
    public class PageChangedEventArgs : EventArgs
    {
        public PageType Page { get; set; }

        public PageChangedEventArgs(PageType page)
        {
            Page = page;
        }
    }
}
