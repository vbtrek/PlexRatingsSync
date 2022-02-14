using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync
{
  public static class EnumHelper
  {
    public static List<EnumValue> GetAll(Type enumType)
    {
      if (!enumType.IsEnum)
        throw new ArgumentException("Enumeration type is expected.");

      List<EnumValue> list = new List<EnumValue>();

      foreach (object value in Enum.GetValues(enumType))
      {
        string enumName = null;

        string enumDescription = EnumDescription(value as Enum);

        if (enumName == null)
          enumName = Enum.GetName(enumType, value);

        EnumValue enumDetails = new EnumValue
        {
          Key = ((int)value),
          Name = enumName,
          Description = enumDescription
        };

        list.Add(enumDetails);
      }

      return list;
    }

    public static List<EnumValue> GetAll<TEnum>()
        where TEnum : struct
    {
      Type enumerationType = typeof(TEnum);

      return GetAll(enumerationType);
    }

    public static string EnumDescription(Enum value)
    {
      try
      {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(DescriptionAttribute),
            false);

        if (attributes != null &&
            attributes.Length > 0)
          return attributes[0].Description;
        else
          return value.ToString();
      }
      catch
      {
        return String.Empty;
      }
    }

    public class EnumValue
    {
      public int Key { get; set; }

      public string Name { get; set; }

      public string Description { get; set; }

      public string DisplayValue
      {
        get { return Description != null ? Description : Name; }
      }

      public override string ToString()
      {
        return DisplayValue;
      }
    }
  }
}
