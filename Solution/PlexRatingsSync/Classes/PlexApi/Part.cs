using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Part")]
  public class Part
  {
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "key")]
    public string Key { get; set; }

    [XmlAttribute(AttributeName = "duration")]
    public int Duration { get; set; }

    [XmlAttribute(AttributeName = "file")]
    public string File { get; set; }

    [XmlAttribute(AttributeName = "size")]
    public int Size { get; set; }

    [XmlAttribute(AttributeName = "container")]
    public string Container { get; set; }

    [XmlAttribute(AttributeName = "hasThumbnail")]
    public bool HasThumbnail { get; set; }

    public void ReadFromXmlFragment(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      Id = reader.GetAttributeValue<int>("id");
      Key = reader.GetAttributeValue<string>("key");
      Duration = reader.GetAttributeValue<int>("duration");
      File = reader.GetAttributeValue<string>("file");
      Size = reader.GetAttributeValue<int>("size");
      Container = reader.GetAttributeValue<string>("container");
      HasThumbnail = reader.GetAttributeValue<bool>("hasThumbnail");

      if (reader.IsEmpty()) return;

      reader.Read();

      while (!reader.EOF)
      {
        if (reader.IsStartElement())
        { }
        else
        {
          reader.Read();
          break;
        }
      }
    }
  }
}
