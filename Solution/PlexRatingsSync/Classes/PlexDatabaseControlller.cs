using DS.Library.MessageHandling;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexDatabaseControlller : IPlexDatabaseControlller, IDisposable
  {
    private SQLiteConnection _DbConnection = null;

    public PlexDatabaseControlller(string database)
    {
      if (!string.IsNullOrWhiteSpace(database))
      {
        _DbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", database));

        try
        {
          _DbConnection.Open();
        }
        catch (Exception)
        {
          _DbConnection = null;
        }
      }
      else
      {
        _DbConnection = null;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Dispose of objects in MyClass only
        if (_DbConnection != null) _DbConnection.Close();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public bool IsDbConnected => _DbConnection != null;

    public List<T> ReadPlexAndMap<T>(string sql)
        where T : new()
    {
      MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
          string.Format("Executing and mapping SQL{0}{1}",
          Environment.NewLine, sql));

      List<T> data = new List<T>();

      using (SQLiteCommand command = new SQLiteCommand(sql, _DbConnection))
      {
        using (SQLiteDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            T item = new T();
            ReadFields(reader, item);
            data.Add(item);
          }
        }
      }

      return data;
    }

    public void ExecutePlexSql(string sql)
    {
      MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
          string.Format("Executing SQL{0}{1}",
          Environment.NewLine, sql));

      using (SQLiteCommand command = new SQLiteCommand(sql, _DbConnection))
      {
        command.ExecuteNonQuery();
      }
    }

    public dynamic ReadPlexValue(string sql)
    {
      MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
          string.Format("Reading SQL Value{0}{1}",
          Environment.NewLine, sql));

      dynamic result = null;

      using (SQLiteCommand command = new SQLiteCommand(sql, _DbConnection))
      {
        using (SQLiteDataReader reader = command.ExecuteReader())
        {
          if (reader.HasRows)
          {
            reader.Read();

            if (reader.IsDBNull(0))
              result = null;
            else
              result = reader[0];
          }
        }
      }
      return result;
    }

    public bool RecordsExists(string sql)
    {
      bool recordExists = false;

      MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
          string.Format("Executing SQL{0}{1}",
          Environment.NewLine, sql));

      using (SQLiteCommand command = new SQLiteCommand(sql, _DbConnection))
      {
        using (SQLiteDataReader reader = command.ExecuteReader())
        {
          if (reader.HasRows)
          {
            recordExists = true;
          }
        }
      }

      return recordExists;
    }

    private void ReadFields(SQLiteDataReader reader, object obj)
    {
      Type type = obj.GetType();
      PropertyInfo[] properties = type.GetProperties();

      foreach (PropertyInfo property in properties)
      {
        string name = property.Name;
        int fieldPos = reader.GetOrdinal(name);

        if (fieldPos >= 0 && !reader.IsDBNull(fieldPos))
        {
          dynamic fieldValue = reader[name];
          property.SetValue(obj, fieldValue, null);
        }
      }
    }
  }
}
