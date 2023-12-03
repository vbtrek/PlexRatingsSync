using System.Collections.Generic;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "MediaContainer")]
  public class MediaContainer
  {
    [XmlElement(ElementName = "Track")]
    public List<Track> Track { get; set; }

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
  }
}
