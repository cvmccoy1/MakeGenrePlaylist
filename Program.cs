using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeGenrePlaylist
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            const string path = @"\\MCCOY-NAS\Public\All Family Stuff\Vana's Music\";
            const string searchPattern = "*.m4a|*.mp3|*.mp4|*.wav|*.wma|*.aac";
            const string fileProperty = "Genre";
            const string filePropertyValue = "Romance";

            Console.Write("Finding All Music Files...");
            ArrayList musicFiles = FileHelper.GetFilesBySearchPattern(path, searchPattern, SearchOption.AllDirectories);
            Console.WriteLine($"Found {musicFiles.Count} Files!");

            Console.Write($"Finding All {filePropertyValue} Files...");
            List<Dictionary<string, string>> genreFiles = FileHelper.GetFilePropertiesBySpecificProperty(musicFiles, fileProperty, filePropertyValue);
            Console.WriteLine($"Found {genreFiles.Count} Files!");

            Console.Write("Creating Play List File...");
            XmlHelper.CreatePlaylist("Romance", Path.Combine(path, "Romance.wpl"), genreFiles);
            Console.WriteLine("Done!");
        }
    }
}
