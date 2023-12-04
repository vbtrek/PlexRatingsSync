using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "MediaContainer")]
  [XmlInclude(typeof(Track))]
  [XmlInclude(typeof(Media))]
  [XmlInclude(typeof(Part))]
  public class MediaContainer
  {
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

    public void ReadFromXml(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      Size = reader.GetAttributeValue<int>("size");
      Identifier = reader.GetAttributeValue<string>("identifier");
      MediaTagPrefix = reader.GetAttributeValue<string>("mediaTagPrefix");
      MediaTagVersion= reader.GetAttributeValue<int>("mediaTagVersion");
      Tracks = new List<Track>();

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

  // TODO_DS1 Move to an XmlManager class
  public static class XmlManager
  {
    public static valueT GetAttributeValue<valueT>(this XmlReader reader, string attributeName)
    {
      try
      {
        var value = reader.GetAttribute(attributeName);

        if (value == null)
          return default(valueT);

        if (typeof(valueT).IsAssignableFrom(typeof(bool)))
        {
          if (value == "1" || value.ToLower() == "true")
            return (valueT)Convert.ChangeType(true, typeof(valueT));

          return (valueT)Convert.ChangeType(false, typeof(valueT));
        }

        return (valueT)Convert.ChangeType(value, typeof(valueT));
      }
      catch
      {
        return default(valueT);
      }
    }

    public static bool IsEmpty(this XmlReader reader)
    {
      if (reader.IsEmptyElement)
      {
        reader.Read();

        return true;
      }

      return false;
    }
  }
}
