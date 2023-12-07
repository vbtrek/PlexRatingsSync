using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using DS.PlexRatingsSync.Managers;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Directory")]
  public class Directory
  {
    [XmlElement(ElementName = "Location")]
    public List<Location> Location { get; set; }

    [XmlAttribute(AttributeName = "allowSync")]
    public int AllowSync { get; set; }

    [XmlAttribute(AttributeName = "art")]
    public string Art { get; set; }

    [XmlAttribute(AttributeName = "composite")]
    public string Composite { get; set; }

    [XmlAttribute(AttributeName = "filters")]
    public int Filters { get; set; }

    [XmlAttribute(AttributeName = "refreshing")]
    public int Refreshing { get; set; }

    [XmlAttribute(AttributeName = "thumb")]
    public string Thumb { get; set; }

    [XmlAttribute(AttributeName = "key")]
    public int Key { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "title")]
    public string Title { get; set; }

    [XmlAttribute(AttributeName = "agent")]
    public string Agent { get; set; }

    [XmlAttribute(AttributeName = "scanner")]
    public string Scanner { get; set; }

    [XmlAttribute(AttributeName = "language")]
    public string Language { get; set; }

    [XmlAttribute(AttributeName = "uuid")]
    public string Uuid { get; set; }

    [XmlAttribute(AttributeName = "updatedAt")]
    public int UpdatedAt { get; set; }

    [XmlAttribute(AttributeName = "createdAt")]
    public int CreatedAt { get; set; }

    [XmlAttribute(AttributeName = "scannedAt")]
    public int ScannedAt { get; set; }

    [XmlAttribute(AttributeName = "content")]
    public int Content { get; set; }

    [XmlAttribute(AttributeName = "directory")]
    public int IsDirectory { get; set; }

    [XmlAttribute(AttributeName = "contentChangedAt")]
    public int ContentChangedAt { get; set; }

    [XmlAttribute(AttributeName = "hidden")]
    public int Hidden { get; set; }

    public void ReadFromXmlFragment(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      AllowSync = reader.GetAttributeValue<int>("allowSync");
      Art = reader.GetAttributeValue<string>("art");
      Composite = reader.GetAttributeValue<string>("composite");
      Filters = reader.GetAttributeValue<int>("Filters");
      Refreshing = reader.GetAttributeValue<int>("refreshing");
      Thumb = reader.GetAttributeValue<string>("thumb");
      Key = reader.GetAttributeValue<int>("key");
      Type = reader.GetAttributeValue<string>("type");
      Title = reader.GetAttributeValue<string>("title");
      Agent = reader.GetAttributeValue<string>("agent");
      Scanner = reader.GetAttributeValue<string>("scanner");
      Language = reader.GetAttributeValue<string>("language");
      Uuid = reader.GetAttributeValue<string>("uuid");
      UpdatedAt = reader.GetAttributeValue<int>("updatedAt");
      CreatedAt = reader.GetAttributeValue<int>("createdAt");
      ScannedAt = reader.GetAttributeValue<int>("scannedAt");
      Content = reader.GetAttributeValue<int>("content");
      IsDirectory = reader.GetAttributeValue<int>("directory");
      ContentChangedAt = reader.GetAttributeValue<int>("contentChangedAt");
      Hidden = reader.GetAttributeValue<int>("hidden");
      Location = new List<Location>();

      if (reader.IsEmpty()) return;

      reader.Read();

      while (!reader.EOF)
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "Location":
              var location = new Location();
              location.ReadFromXmlFragment(reader);
              Location.Add(location);
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
