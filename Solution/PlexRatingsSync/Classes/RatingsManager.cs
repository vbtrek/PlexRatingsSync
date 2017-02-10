using DS.Library.MessageHandling;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
    public class RatingsManager
    {
        public static int? PlexRatingFromFile(string file)
        {
            try
            {
                uint? fileRating = null;
                ShellFile so = ShellFile.FromFilePath(file);

                if (so.Properties.System.Rating.Value != null)
                    fileRating = (uint)so.Properties.System.Rating.Value;

                if (fileRating == null)
                    return null;

                if (fileRating < 1)
                    return null;
                else if (fileRating < 13)
                    return 2;
                else if (fileRating < 38)
                    return 4;
                else if (fileRating < 63)
                    return 6;
                else if (fileRating < 87)
                    return 8;
                else
                    return 10;
            }
            catch (Exception ex)
            {
                MessageManager.Instance.ExceptionWrite(new RatingsManager(), ex);
                return null;
            }
        }

        public static int? FileRatingFromPlexRating(int? plexRating)
        {
            try
            {
                switch (plexRating)
                {
                    case 2:
                        return 1;
                    case 4:
                        return 25;
                    case 6:
                        return 50;
                    case 8:
                        return 75;
                    case 10:
                        return 99;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                MessageManager.Instance.ExceptionWrite(new RatingsManager(), ex);
                return null;
            }
        }
    }
}
