using System.Collections.Generic;

namespace DS.PlexRatingsSync
{
  public interface IPlexDatabaseControlller
  {
    bool IsDbConnected { get; }

    void Dispose();

    void ExecutePlexSql(string sql);

    List<T> ReadPlexAndMap<T>(string sql) where T : new();

    dynamic ReadPlexValue(string sql);

    bool RecordsExists(string sql);
  }
}
