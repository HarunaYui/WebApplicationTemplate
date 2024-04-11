using MySqlConnector;

namespace WebApplicationTemplate.AppDB;

/// <summary>
/// 数据库注入
/// </summary>
/// <param name="connectionString"></param>
public class AppDB(string connectionString) : IDisposable
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    public MySqlConnection Connection { get; set; } = new (connectionString);

    /// <summary>
    /// 资源释放
    /// </summary>
    public void Dispose()
    {
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }
}

