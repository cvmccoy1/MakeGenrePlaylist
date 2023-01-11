using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MakeGenrePlaylist
{
    public static class XmlHelper
    {
        /// <summary>Creates a music play list file.</summary>
        /// <param name="name">The name of the play list.</param>
        /// <param name="filePath">The file's full pathname.</param>
        /// <param name="musicFiles">The list of music files to include in the play list.</param>
        public static void CreatePlaylist(string name, string filePath, ArrayList musicFiles)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine("<?wpl version=\"1.0\"?>");
                    writer.WriteLine("<smil>");
                    writer.WriteLine("\t<head>");
                    writer.WriteLine("\t\t<meta name = \"Generator\" content = \"Microsoft Windows Media Player -- 12.0.19041.1566\" />");
                    writer.WriteLine($"\t\t<meta name=\"ItemCount\" content=\"{ musicFiles.Count}\" />");
                    writer.WriteLine($"\t\t<title>{name}</title>");
                    writer.WriteLine("\t</head>");
                    writer.WriteLine("\t<body>");
                    writer.WriteLine("\t\t<seq>");

                    foreach (string file in musicFiles)
                    {
                        writer.Write($"\t\t\t<media src=\"{Escape(file)}\" ");
                        writer.Write($"albumTitle=\"{Escape(FileHelper.GetExtendedFileProperty(file, "Album"))}\" ");
                        writer.Write($"albumArtist=\"{Escape(FileHelper.GetExtendedFileProperty(file, "Contributing artists"))}\" ");
                        writer.Write($"trackTitle=\"{Escape(FileHelper.GetExtendedFileProperty(file, "Title"))}\" ");
                        writer.Write($"trackArtist=\"{Escape(FileHelper.GetExtendedFileProperty(file, "Authors"))}\" ");
                        string length = FileHelper.GetExtendedFileProperty(file, "Length");
                        TimeSpan timeSpan = TimeSpan.ParseExact(length, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
                        writer.Write($"duration=\"{timeSpan.TotalMilliseconds}\" ");
                        writer.WriteLine("/>");
                    }

                    writer.WriteLine("\t\t</seq>");
                    writer.WriteLine("\t</body>");
                    writer.WriteLine("</smil>");
                }
            }
        }

        /// <summary>
        ///   <para>
        /// Escapes the specified string
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The string with the escape sequences.</returns>
        private static string Escape(string str)
        {
            string escapeStr = str.Replace("&", "&amp;")
                                  .Replace("<", "&lt;")
                                  .Replace(">", "&gt;")
                                  .Replace("'", "&apos;")
                                  .Replace("\"", "&quot;");
            return escapeStr;
        }
    }
}
