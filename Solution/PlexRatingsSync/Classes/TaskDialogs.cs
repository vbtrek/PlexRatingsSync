using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class TaskDialogs
  {
    private TaskDialog _TaskDialog = null;

    private RatingsClashResult _RatingsClashReturn = RatingsClashResult.Cancel;

    public RatingsClashResult RatingsClash(string file, int? plexRating, int? fileRating)
    {
      // Error dialog
      _TaskDialog = new TaskDialog
      {
        DetailsExpanded = false,
        Cancelable = true,
        Icon = TaskDialogStandardIcon.Warning,
        StandardButtons = TaskDialogStandardButtons.Cancel,

        Caption = "Mismatched Ratings",
        InstructionText = "There is a mismatch between the ratings for this file:",
        Text = file + "\n\nChoose which rating you would like to keep:"
      };

      string plexRatingDisplay = plexRating == null ? "UnSet" : plexRating.ToString();

      string plexButtonText = "Plex " + plexRatingDisplay + " Stars\nClick here to overwrite the file rating with the Plex rating";

      if (Settings.SyncSource == SyncSources.ITunesLibrary)
        plexButtonText = "Plex " + plexRatingDisplay + " Stars\nClick here to overwrite the iTunes rating with the Plex rating";

      string fileRatingDisplay = fileRating == null ? "UnSet" : fileRating.ToString();

      string fileButtonText = "File " + fileRatingDisplay + " Stars\nClick here to overwrite the Plex rating with the file rating";

      if (Settings.SyncSource == SyncSources.ITunesLibrary)
        fileButtonText = "iTunes " + fileRatingDisplay + " Stars\nClick here to overwrite the Plex rating with the iTunes rating";

      TaskDialogCommandLink plexButton = new TaskDialogCommandLink("plexButton", plexButtonText);
      
      plexButton.Click += new EventHandler(plexButton_Click);

      TaskDialogCommandLink fileButton = new TaskDialogCommandLink("fileButton", fileButtonText);
      
      fileButton.Click += new EventHandler(fileButton_Click);

      _TaskDialog.Controls.Add(plexButton);

      _TaskDialog.Controls.Add(fileButton);

      _TaskDialog.Show();

      _TaskDialog = null;

      return _RatingsClashReturn;
    }

    private void plexButton_Click(object sender, EventArgs e)
    {
      _RatingsClashReturn = RatingsClashResult.UsePlex;

      if (_TaskDialog != null) _TaskDialog.Close();
    }

    private void fileButton_Click(object sender, EventArgs e)
    {
      _RatingsClashReturn = RatingsClashResult.UseFileOrItunes;

      if (_TaskDialog != null) _TaskDialog.Close();
    }
  }
}
