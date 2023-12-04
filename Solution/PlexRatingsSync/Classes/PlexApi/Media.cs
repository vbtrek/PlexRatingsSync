using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Media")]
  public class Media
  {
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "duration")]
    public int Duration { get; set; }

    [XmlAttribute(AttributeName = "bitrate")]
    public int Bitrate { get; set; }

    [XmlAttribute(AttributeName = "audioChannels")]
    public int AudioChannels { get; set; }

    [XmlAttribute(AttributeName = "audioCodec")]
    public string AudioCodec { get; set; }

    [XmlAttribute(AttributeName = "container")]
    public string Container { get; set; }

    [XmlElement(ElementName = "Part")]
    public Part Part { get; set; }

    public void ReadFromXmlFragment(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      Id = reader.GetAttributeValue<int>("id");
      Duration = reader.GetAttributeValue<int>("duration");
      Bitrate = reader.GetAttributeValue<int>("bitrate");
      AudioChannels = reader.GetAttributeValue<int>("audioChannels");
      AudioCodec = reader.GetAttributeValue<string>("audioCodec");
      Container = reader.GetAttributeValue<string>("container");

      if (reader.IsEmpty()) return;

      reader.Read();

      while (!reader.EOF)
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "Part":
              Part = new Part();
              Part.ReadFromXmlFragment(reader);
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
