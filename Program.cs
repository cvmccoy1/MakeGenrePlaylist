using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace MakeGenrePlaylist
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand
            {
                new Option<string>(
                    new [] { "--fullpath", "-f" },
                    "Specify the path to search."),
                new Option<string>(
                    new [] { "--property", "-p" },
                    "Specify the property to filter on."),
                new Option<string>(
                    new [] { "--value", "-v" },
                    "Specify the property's value.")
            };
            rootCommand.Description = "App to search for files with a specific property and creates a Windows playlist file (.wpl)";

            rootCommand.Handler = CommandHandler.Create<string, string, string>(Execute);
            // Parse the incoming args and invoke the handler
            return rootCommand.Invoke(args);
        }

        private static void Execute(string fullpath, string property, string value)
        {
            string path = String.IsNullOrWhiteSpace(fullpath) ? @"\\MCCOY-NAS\Public\All Family Stuff\Vana's Music\" : fullpath;
            string fileProperty = String.IsNullOrWhiteSpace(property) ? "Genre" : property;
            string filePropertyValue = String.IsNullOrWhiteSpace(value) ? "Romance" : value;

            List<string> searchExt = new List<string>()
            {
                ".m4a", ".mp3",  ".mp4", ".wav", ".wma", ".aac"
            };

            Console.Write("Finding All Music Files...");
            ArrayList musicFiles = FileHelper.GetFilesBySearchPattern(path, searchExt);
            Console.WriteLine($"Found {musicFiles.Count} Files!");

            Console.Write($"Finding All {filePropertyValue} Files...");
            List<Dictionary<string, string>> genreFiles = FileHelper.GetFilePropertiesBySpecificProperty(musicFiles, fileProperty, filePropertyValue);
            Console.WriteLine($"Found {genreFiles.Count} Files!");

            string playlistName = filePropertyValue.Trim().Replace(" ", "_");
            Console.Write($"Creating Play List File {playlistName}...");
            XmlHelper.CreatePlaylist($"{playlistName}", Path.Combine(path, $"{playlistName}.wpl"), genreFiles);
            Console.WriteLine("Done!");
        }
    }
}
