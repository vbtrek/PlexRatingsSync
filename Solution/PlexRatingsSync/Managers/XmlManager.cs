using System;
using System.Xml;

namespace DS.PlexRatingsSync.Managers
{
  public static class XmlManager
  {
    public static valueT GetAttributeValue<valueT>(this XmlReader reader, string attributeName)
    {
      try
      {
        var value = reader.GetAttribute(attributeName);

        if (value == null)
          return default(valueT);

        if (typeof(valueT).IsAssignableFrom(typeof(bool)))
        {
          if (value == "1" || value.ToLower() == "true")
            return (valueT)Convert.ChangeType(true, typeof(valueT));

          return (valueT)Convert.ChangeType(false, typeof(valueT));
        }

        return (valueT)Convert.ChangeType(value, typeof(valueT));
      }
      catch
      {
        return default(valueT);
      }
    }

    public static bool IsEmpty(this XmlReader reader)
    {
      if (reader.IsEmptyElement)
      {
        reader.Read();

        return true;
      }

      return false;
    }
  }
}
