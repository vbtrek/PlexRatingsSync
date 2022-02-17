﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync
{
  public static class PlaylistManager
  {
    public static void SyncPlaylists(SyncArgs args)
    {
      if (args.Worker.CancellationPending) return;

      if (!Settings.SyncPlaylists) return;

      // Set progress max
      args.ReportProgress(Settings.ChosenPlaylists.Count);

      args.ReportProgress(SyncArgs.ProgressType.ResetCounters);

      args.ReportProgress("Syncing Playlists...");

      foreach (var playlist in args.ItunesData.ItunesPlaylists)
      {
        if (args.Worker.CancellationPending) return;

        if (!Settings.ChosenPlaylists.Contains(playlist.FullPlaylistName)) continue;

        args.ReportProgress($"Syncing Playlist \"{playlist.FullPlaylistName}\"...");

        bool isUpdate = false;

        int? playlistId = GetPlexPlaylistId(args, playlist);

        if (playlistId != null && playlistId > 0) isUpdate = true;

        playlistId = CreateOrClearPlexPlaylist(args, playlistId, playlist);

        // Ensure there is a metadata_item_accounts record for each playlist
        CreateMetadataItemAccounts(args, playlistId);

        // Insert the items into the playlist
        int addDuration = InsertTracksInPlexPlaylist(args, playlistId, playlist);

        // Update totals or remove empty playlist
        UpdateOrRemovePlexPlaylist(args, playlistId, playlist, addDuration);

        if (isUpdate)
          args.ReportProgress(SyncArgs.ProgressType.IncrementUpdatedCount);
        else
          args.ReportProgress(SyncArgs.ProgressType.IncrementNewCount);

        args.ReportProgress(SyncArgs.ProgressType.IncrementProgressBar);
      }
    }

    private static int? GetPlexPlaylistId(SyncArgs args, ItunesPlaylist playlist)
    {
      int? playlistId = null;

      string sql = $@"SELECT id
FROM metadata_items
WHERE title = '{playlist.FullPlaylistName}'
AND metadata_type = 15";    //15=PLAYLIST_TYPE
      
      playlistId = (int?)args.PlexDb.ReadPlexValue(sql);

      return playlistId;
    }

    /// <summary>
    /// Creates a playlist in Plex if playlistId is invalid, otherwise clears the existing playlist in Plex ready for re-population.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="playlistId"></param>
    /// <param name="playlist"></param>
    /// <returns>The Plex Playlist ID</returns>
    private static int? CreateOrClearPlexPlaylist(SyncArgs args, int? playlistId, ItunesPlaylist playlist)
    {
      string sql = string.Empty;

      if (playlistId != null && playlistId > 0)
      {
        // Delete playlist entries (ready to rebuild)
        sql = $"DELETE FROM play_queue_generators WHERE playlist_id = {playlistId}";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif

        // Update the playlist back to blank
        sql = $@"
UPDATE metadata_items
SET media_item_count = 0, duration = 0, 
updated_at = datetime('now'), extra_data = 'pv%3AdurationInSeconds=1&pv%3AsectionIDs=1'
WHERE id = {playlistId}";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif
      }
      else
      {
        // Create playlist
        sql = $@"
INSERT INTO metadata_items
([guid], [metadata_type], [media_item_count], [title], [title_sort], [index], [absolute_index], [duration], [added_at], [created_at], [updated_at], [extra_data])
VALUES
('com.plexapp.agents.none://{Guid.NewGuid()}', 15, 0, '{playlist.FullPlaylistName}', '{playlist.FullPlaylistName}', 
0, 10, 0, datetime('now'), datetime('now'), datetime('now'), 'pv%3AdurationInSeconds=1&pv%3AsectionIDs=1')";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif

        playlistId = GetPlexPlaylistId(args, playlist);
      }

      return playlistId;
    }

    /// <summary>
    /// Ensure there is a metadata_item_accounts record for each playlist
    /// </summary>
    /// <param name="args"></param>
    /// <param name="playlistId"></param>
    private static void CreateMetadataItemAccounts(SyncArgs args, int? playlistId)
    {
      string sql = $@"SELECT id
FROM metadata_item_accounts
WHERE account_id = {Settings.PlexAccountId} AND metadata_item_id = {playlistId}";

      int? id = (int?)args.PlexDb.ReadPlexValue(sql);

      if (id == null)
      {
        sql = $@"
INSERT INTO metadata_item_accounts (account_id, metadata_item_id)
VALUES ({Settings.PlexAccountId}, {playlistId})";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif
      }
    }

    /// <summary>
    /// Inserts tracks from the iTunes playlirt into the Plex playlist.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="playlistId"></param>
    /// <param name="playlist"></param>
    /// <returns>Total track duration</returns>
    private static int InsertTracksInPlexPlaylist(SyncArgs args, int? playlistId, ItunesPlaylist playlist)
    {
      int addDuration = 0;

      int orderIncrement = 1000;

      string sql = $"SELECT MAX([order]) FROM play_queue_generators WHERE playlist_id = {playlistId}";

      double? currentOrder = args.PlexDb.ReadPlexValue(sql);

      currentOrder = currentOrder == null ? orderIncrement : currentOrder;

      foreach (var item in playlist.Tracks)
      {
        // Lookup the file in the DB to get it's id
        sql = $@"
SELECT MI.metadata_item_id
FROM media_parts AS MP
INNER JOIN media_items AS MI ON MI.id = MP.media_item_id
WHERE MP.file = '{item.ProperLocation.Replace("'", "''")}' COLLATE NOCASE";

        long? dbItemID = args.PlexDb.ReadPlexValue(sql) as long?;

        if (dbItemID != null)
        {
          // Lookup the duration in the DB
          sql = $@"
SELECT MP.duration
FROM media_parts AS MP
INNER JOIN media_items AS MI ON MI.id = MP.media_item_id
WHERE MI.metadata_item_id = {dbItemID}";

          long? dbDuration = args.PlexDb.ReadPlexValue(sql) as long?;

          if (dbDuration == null) dbDuration = 0;

          // Create a new playlist entry
          sql = $@"
INSERT INTO play_queue_generators
([playlist_id], [metadata_item_id], [order], [created_at], [updated_at], [uri])
VALUES
({playlistId}, {dbItemID}, {currentOrder}, datetime('now'), datetime('now'), '')";

#if DEBUG
          Debug.Print(sql);
#else
          args.PlexDb.ExecutePlexSql(sql);
#endif

          currentOrder += orderIncrement;

          addDuration += (int)(dbDuration / 1000);
        }
      }

      return addDuration;
    }

    /// <summary>
    /// Updates playlist totals, or deletes the playlist if there are no tracks.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="playlistId"></param>
    /// <param name="playlist"></param>
    /// <param name="addDuration"></param>
    private static void UpdateOrRemovePlexPlaylist(SyncArgs args, int? playlistId, ItunesPlaylist playlist, int addDuration)
    {
      string sql = string.Empty;

      if (playlist.Tracks.Count == 0 && Settings.RemoveEmptyPlaylists)
      {
        // Remove the playlist
        sql = $@"DELETE FROM metadata_item_accounts WHERE metadata_item_id = {playlistId}";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif

        sql = $@"DELETE FROM metadata_items WHERE id = {playlistId}";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif
      }
      else
      {
        // Update the playlists info
        sql = $@"
UPDATE metadata_items
SET duration = {addDuration},
media_item_count = {playlist.Tracks.Count}
WHERE id = {playlistId}";

#if DEBUG
        Debug.Print(sql);
#else
        args.PlexDb.ExecutePlexSql(sql);
#endif
      }
    }
  }
}
