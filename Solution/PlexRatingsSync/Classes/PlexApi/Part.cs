using System.Collections.Generic;
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
    public int HasThumbnail { get; set; }
  }
}
