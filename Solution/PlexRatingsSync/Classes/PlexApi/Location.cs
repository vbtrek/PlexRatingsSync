using System.ComponentModel;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using DS.PlexRatingsSync.Managers;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Location")]
  public class Location
  {
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "path")]
    public string Path { get; set; }

    public void ReadFromXmlFragment(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      Id = reader.GetAttributeValue<int>("id");
      Path = reader.GetAttributeValue<string>("path");

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
