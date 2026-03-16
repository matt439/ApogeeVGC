using Microsoft.Data.Sqlite;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Provides read access to the SQLite-based batch cache for equivalence tests.
/// Each format has a single .db file containing all battle fixtures and logs.
/// Thread-safe: uses a connection pool (one connection per thread).
/// </summary>
public sealed class BatchCacheDb : IDisposable
{
    private readonly string _connectionString;
    private readonly ThreadLocal<SqliteConnection> _connections = new(trackAllValues: true);

    public BatchCacheDb(string dbPath)
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadOnly,
            Cache = SqliteCacheMode.Shared,
        }.ToString();
    }

    private SqliteConnection GetConnection()
    {
        if (_connections.Value is { State: System.Data.ConnectionState.Open } conn)
            return conn;

        var newConn = new SqliteConnection(_connectionString);
        newConn.Open();

        using var pragma = newConn.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode = WAL; PRAGMA cache_size = -8000;";
        pragma.ExecuteNonQuery();

        _connections.Value = newConn;
        return newConn;
    }

    /// <summary>
    /// Returns true if a battle with the given id exists in the database.
    /// </summary>
    public bool Exists(int id)
    {
        var conn = GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM battles WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        return cmd.ExecuteScalar() != null;
    }

    /// <summary>
    /// Reads the fixture JSON and log for a given battle index.
    /// Returns null if the battle is not found.
    /// </summary>
    public (string FixtureJson, string Log)? GetBattle(int id)
    {
        var conn = GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT fixture_json, log FROM battles WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return (reader.GetString(0), reader.GetString(1));
    }

    /// <summary>
    /// Reads a metadata value by key. Returns null if not found.
    /// </summary>
    public string? GetMetadata(string key)
    {
        var conn = GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT value FROM metadata WHERE key = $key";
        cmd.Parameters.AddWithValue("$key", key);
        return cmd.ExecuteScalar() as string;
    }

    /// <summary>
    /// Sets a metadata value by key (upsert).
    /// Requires a writable database — opens a separate connection.
    /// </summary>
    public void SetMetadata(string key, string value)
    {
        using var conn = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = new SqliteConnectionStringBuilder(_connectionString).DataSource,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString());
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS metadata (key TEXT PRIMARY KEY, value TEXT NOT NULL); " +
                          "INSERT OR REPLACE INTO metadata (key, value) VALUES ($key, $value)";
        cmd.Parameters.AddWithValue("$key", key);
        cmd.Parameters.AddWithValue("$value", value);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns the total number of battles in the database.
    /// </summary>
    public int Count()
    {
        var conn = GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM battles";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Dispose()
    {
        foreach (var conn in _connections.Values)
        {
            conn?.Dispose();
        }
        _connections.Dispose();
    }
}
