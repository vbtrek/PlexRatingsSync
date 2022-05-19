using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync
{
	[XmlRoot(ElementName = "plist")]
	public class ItunesLibrary
	{
		[XmlElement(ElementName = "dict")]
		public ItunesDict Dict { get; set; }

		[XmlAttribute(AttributeName = "version")]
		public double Version { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "dict")]
	public class ItunesDict
	{
		[XmlIgnore]
		public List<KeyValuePair<string, string>> KeyValuePairs = new List<KeyValuePair<string, string>>();

		/*
		[XmlElement(ElementName = "key")]
		public List<string> Key { get; set; }

		[XmlElement(ElementName = "integer")]
		public List<decimal> Integer { get; set; }

		[XmlElement(ElementName = "string")]
		public List<string> String { get; set; }
		*/
		public void AddKeyValuePair(string key, string value)
    {
			KeyValuePairs.Add(new KeyValuePair<string, string>(key, value));
    }

		[XmlElement(ElementName = "dict")]
		public ItunesDict Dict { get; set; }

		[XmlElement(ElementName = "array")]
		public ItunesArray Array { get; set; }
	}

	[XmlRoot(ElementName = "array")]
	public class ItunesArray
	{
		[XmlElement(ElementName = "dict")]
		public List<ItunesDict> Dict { get; set; }
	}
}
