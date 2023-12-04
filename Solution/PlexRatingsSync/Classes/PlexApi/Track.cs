using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Track")]
  public class Track
  {
    [XmlAttribute(AttributeName = "ratingKey")]
    public int RatingKey { get; set; }

    [XmlAttribute(AttributeName = "key")]
    public string Key { get; set; }

    [XmlAttribute(AttributeName = "parentRatingKey")]
    public int ParentRatingKey { get; set; }

    [XmlAttribute(AttributeName = "grandparentRatingKey")]
    public int GrandparentRatingKey { get; set; }

    [XmlAttribute(AttributeName = "guid")]
    public string Guid { get; set; }

    [XmlAttribute(AttributeName = "parentGuid")]
    public string ParentGuid { get; set; }

    [XmlAttribute(AttributeName = "grandparentGuid")]
    public string GrandparentGuid { get; set; }

    [XmlAttribute(AttributeName = "parentStudio")]
    public string ParentStudio { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "title")]
    public string Title { get; set; }

    [XmlAttribute(AttributeName = "grandparentKey")]
    public string GrandparentKey { get; set; }

    [XmlAttribute(AttributeName = "parentKey")]
    public string ParentKey { get; set; }

    [XmlAttribute(AttributeName = "librarySectionTitle")]
    public string LibrarySectionTitle { get; set; }

    [XmlAttribute(AttributeName = "librarySectionID")]
    public int LibrarySectionID { get; set; }

    [XmlAttribute(AttributeName = "librarySectionKey")]
    public string LibrarySectionKey { get; set; }

    [XmlAttribute(AttributeName = "grandparentTitle")]
    public string GrandparentTitle { get; set; }

    [XmlAttribute(AttributeName = "parentTitle")]
    public string ParentTitle { get; set; }

    [XmlAttribute(AttributeName = "summary")]
    public string Summary { get; set; }

    [XmlAttribute(AttributeName = "index")]
    public int Index { get; set; }

    [XmlAttribute(AttributeName = "parentIndex")]
    public int ParentIndex { get; set; }

    [XmlAttribute(AttributeName = "userRating")]
    public decimal UserRating { get; set; }

    [XmlAttribute(AttributeName = "ratingCount")]
    public int RatingCount { get; set; }

    [XmlAttribute(AttributeName = "parentYear")]
    public int ParentYear { get; set; }

    [XmlAttribute(AttributeName = "thumb")]
    public string Thumb { get; set; }

    [XmlAttribute(AttributeName = "parentThumb")]
    public string ParentThumb { get; set; }

    [XmlAttribute(AttributeName = "grandparentThumb")]
    public string GrandparentThumb { get; set; }

    [XmlAttribute(AttributeName = "duration")]
    public int Duration { get; set; }

    [XmlAttribute(AttributeName = "addedAt")]
    public int AddedAt { get; set; }

    [XmlAttribute(AttributeName = "updatedAt")]
    public int UpdatedAt { get; set; }

    [XmlAttribute(AttributeName = "musicAnalysisVersion")]
    public int MusicAnalysisVersion { get; set; }

    [XmlAttribute(AttributeName = "originalTitle")]
    public string OriginalTitle { get; set; }

    [XmlElement(ElementName = "Media")]
    public Media Media { get; set; }

    [XmlIgnore]
    public string CleanKey
    {
      get
      {
        // Remove the key prefix: example key=/library/metadata/28999
        if (Key.StartsWith("/library/metadata/"))
          return Key.Substring("/library/metadata/".Length);

        return Key;
      }
    }

    public void ReadFromXmlFragment(XmlReader reader)
    {
      reader.MoveToContent();

      // Read node attributes
      RatingKey = reader.GetAttributeValue<int>("ratingKey");
      Key = reader.GetAttributeValue<string>("key");
      ParentRatingKey = reader.GetAttributeValue<int>("parentRatingKey");
      GrandparentRatingKey = reader.GetAttributeValue<int>("grandparentRatingKey");
      Guid = reader.GetAttributeValue<string>("guid");
      ParentGuid = reader.GetAttributeValue<string>("parentGuid");
      GrandparentGuid = reader.GetAttributeValue<string>("grandparentGuid");
      ParentStudio = reader.GetAttributeValue<string>("parentStudio");
      Type = reader.GetAttributeValue<string>("type");
      Title = reader.GetAttributeValue<string>("title");
      GrandparentKey = reader.GetAttributeValue<string>("grandparentKey");
      ParentKey = reader.GetAttributeValue<string>("parentKey");
      LibrarySectionTitle = reader.GetAttributeValue<string>("librarySectionTitle");
      LibrarySectionID = reader.GetAttributeValue<int>("librarySectionID");
      LibrarySectionKey = reader.GetAttributeValue<string>("librarySectionKey");
      GrandparentTitle = reader.GetAttributeValue<string>("grandparentTitle");
      ParentTitle = reader.GetAttributeValue<string>("parentTitle");
      Summary = reader.GetAttributeValue<string>("summary");
      Index = reader.GetAttributeValue<int>("index");
      ParentIndex = reader.GetAttributeValue<int>("parentIndex");
      UserRating = reader.GetAttributeValue<decimal>("userRating");
      RatingCount = reader.GetAttributeValue<int>("ratingCount");
      ParentYear = reader.GetAttributeValue<int>("parentYear");
      Thumb = reader.GetAttributeValue<string>("thumb");
      ParentThumb = reader.GetAttributeValue<string>("parentThumb");
      GrandparentThumb = reader.GetAttributeValue<string>("grandparentThumb");
      Duration = reader.GetAttributeValue<int>("duration");
      AddedAt = reader.GetAttributeValue<int>("addedAt");
      UpdatedAt = reader.GetAttributeValue<int>("updatedAt");
      MusicAnalysisVersion = reader.GetAttributeValue<int>("musicAnalysisVersion");
      OriginalTitle = reader.GetAttributeValue<string>("originalTitle");

      if (reader.IsEmpty()) return;

      reader.Read();

      while (!reader.EOF)
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "Media":
              Media = new Media();
              Media.ReadFromXmlFragment(reader);
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
