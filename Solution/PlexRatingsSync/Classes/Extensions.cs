using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public static class Extensions
  {
    public static string EllipsisString(this string rawString, int maxLength = 30, char delimiter = '\\')
    {
      maxLength -= 3; //account for delimiter spacing

      if (rawString.Length <= maxLength)
      {
        return rawString;
      }

      string final = rawString;
      List<string> parts;

      int loops = 0;
      while (loops++ < 100)
      {
        parts = rawString.Split(delimiter).ToList();
        parts.RemoveRange(parts.Count - 1 - loops, loops);
        if (parts.Count == 1)
        {
          return parts.Last();
        }

        parts.Insert(parts.Count - 1, "...");
        final = string.Join(delimiter.ToString(), parts);
        if (final.Length < maxLength)
        {
          return final;
        }
      }

      return rawString.Split(delimiter).ToList().Last();
    }
  }
}
