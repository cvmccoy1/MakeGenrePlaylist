using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        public static void CreatePlaylist(string name, string filePath, ArrayList musicFiles)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteRaw("<?wpl version=\"1.0\"?>\n");
                writer.WriteStartElement("smil");
                writer.WriteStartElement("head");
                writer.WriteStartElement("meta");
                writer.WriteAttributeString("name", "Generator");
                writer.WriteAttributeString("content", "Microsoft Windows Media Player -- 12.0.19041.1566");
                writer.WriteEndElement();
                writer.WriteStartElement("meta");
                writer.WriteAttributeString("name", "ItemCount");
                writer.WriteAttributeString("content", musicFiles.Count.ToString());
                writer.WriteEndElement();
                writer.WriteElementString("title", name);
                writer.WriteEndElement(); // head
                writer.WriteStartElement("body");
                writer.WriteStartElement("seq");
                foreach (string file in musicFiles)
                {
                    writer.WriteStartElement("media");
                    writer.WriteAttributeString("src", Escape(file));
                    writer.WriteAttributeString("albumTitle", Escape(FileHelper.GetExtendedFileProperty(file, "Album")));
                    writer.WriteAttributeString("albumArtist", Escape(FileHelper.GetExtendedFileProperty(file, "Contributing artists")));
                    writer.WriteAttributeString("trackTitle", Escape(FileHelper.GetExtendedFileProperty(file, "Title")));
                    writer.WriteAttributeString("trackArtist", Escape(FileHelper.GetExtendedFileProperty(file, "Authors")));
                    string lengthInTime = FileHelper.GetExtendedFileProperty(file, "Length");
                    TimeSpan timeSpan = TimeSpan.ParseExact(lengthInTime, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
                    writer.WriteAttributeString("duration", timeSpan.TotalMilliseconds.ToString());
                    writer.WriteEndElement(); // media
                }
                writer.WriteEndElement(); // body
                writer.WriteEndElement(); // seq
                writer.WriteEndElement(); // smil
                writer.WriteEndDocument();
            }
        }

        private static string Escape(string str)
        {
            string escapeStr = str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("&#34;", "&quot;").Replace("'", "&apos;");
            return escapeStr;
        }
    }
}
