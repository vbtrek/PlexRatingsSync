using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DS.PlexRatingsSync.Managers;
using MS.WindowsAPICodePack.Internal;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "MediaContainer")]
  [XmlInclude(typeof(Track))]
  [XmlInclude(typeof(Media))]
  [XmlInclude(typeof(Part))]
  [XmlInclude(typeof(Directory))]
  public class MediaContainer
  {
    // From library/sections call
    [XmlAttribute(AttributeName = "size")]
    public int Size { get; set; }

    [XmlAttribute(AttributeName = "allowSync")]
    public int AllowSync { get; set; }

    [XmlAttribute(AttributeName = "identifier")]
    public string Identifier { get; set; }

    [XmlAttribute(AttributeName = "mediaTagPrefix")]
    public string MediaTagPrefix { get; set; }

    [XmlAttribute(AttributeName = "mediaTagVersion")]
    public int MediaTagVersion { get; set; }

    [XmlElement(ElementName = "Track")]
    public List<Track> Tracks { get; set; }

    // From Identity call
    [XmlAttribute(AttributeName = "machineIdentifier")]
    public string MachineIdentifier { get; set; }

    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }

    [XmlElement(ElementName = "Directory")]
    public List<Directory> Directory { get; set; }

    [XmlAttribute(AttributeName = "title1")]
    public string Title1 { get; set; }

    public static MediaContainer Parse(string xml)
    {
      MediaContainer container = new MediaContainer();

      try
      {
        using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
          container.ReadFromXml(reader);
      }
      catch
      { }

      return container;
    }

    private void ReadFromXml(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      Size = reader.GetAttributeValue<int>("size");
      Identifier = reader.GetAttributeValue<string>("identifier");
      MediaTagPrefix = reader.GetAttributeValue<string>("mediaTagPrefix");
      MediaTagVersion= reader.GetAttributeValue<int>("mediaTagVersion");
      Tracks = new List<Track>();
      Directory = new List<Directory>();

      if (reader.IsEmpty()) return;

      reader.Read();

      while (!reader.EOF)
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "Track":
              var track = new Track();
              track.ReadFromXmlFragment(reader);
              Tracks.Add(track);
              break;

            case "Directory":
              var directory = new Directory();
              directory.ReadFromXmlFragment(reader);
              Directory.Add(directory);
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Read();
          break;
        }
      }
    }
  }
}
