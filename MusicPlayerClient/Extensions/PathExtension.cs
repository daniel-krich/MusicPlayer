using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Extensions
{
    public static class PathExtension
    {
        public static bool HasOneOfExtensions(string path, params string[] extensions)
        {
            foreach(string extension in extensions)
            {
                if(Path.GetExtension(path) == extension)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
