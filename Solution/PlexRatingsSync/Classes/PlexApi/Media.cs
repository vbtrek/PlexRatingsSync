using System.Collections.Generic;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Media")]
  public class Media
  {
    [XmlElement(ElementName = "Part")]
    public Part Part { get; set; }

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
  }
}
