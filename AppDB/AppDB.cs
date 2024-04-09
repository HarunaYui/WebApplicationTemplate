using MySqlConnector;

namespace TestWebApplication.AppDB;

public class AppDB : IDisposable
{
    public MySqlConnection Connection { get; set; }

    public AppDB(string connectionString)
    {
        Connection = new MySqlConnection(connectionString);
    }

    public void Dispose()
    {
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }
}

