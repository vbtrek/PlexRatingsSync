using System.Collections.Generic;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync.Classes.PlexApi
{
  [XmlRoot(ElementName = "Track")]
  public class Track
  {
    [XmlElement(ElementName = "Media")]
    public Media Media { get; set; }

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
    public object Summary { get; set; }

    [XmlAttribute(AttributeName = "index")]
    public int Index { get; set; }

    [XmlAttribute(AttributeName = "parentIndex")]
    public int ParentIndex { get; set; }

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
  }
}
