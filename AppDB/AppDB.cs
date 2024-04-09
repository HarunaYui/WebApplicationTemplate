using MySqlConnector;

namespace WebApplicationTemplate.AppDB;

public class AppDB(string connectionString) : IDisposable
{
    public MySqlConnection Connection { get; set; } = new MySqlConnection(connectionString);

    public void Dispose()
    {
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }
}

