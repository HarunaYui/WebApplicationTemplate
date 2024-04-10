using Dapper;
using Dapper.Contrib.Extensions;
using WebApplicationTemplate.AppDB.Extensions;

namespace WebApplicationTemplate.AppDB;
public class Query(AppDB db)
{
    public AppDB Db { get; set; } = db;

    public string _defaultId = "ID";

    public async Task<bool> CheckTable(string table)
    {
        await Db.Connection.OpenAsync();
        var result = await Db.Connection.ExecuteScalarAsync($"SHOW TABLES LIKE '{table}'");
        await Db.Connection.CloseAsync();
        return result != null;
    }

    public async Task<bool> CreateTable<T>(string table)
    {
        if (await CheckTable(table))
            return true;
        await Db.Connection.OpenAsync();
        var properties = typeof(T).GetProperties().Select(c =>
        {
            var key = c.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var _size = c.GetCustomAttributes(typeof(SizeAttribute), false).FirstOrDefault();
            var size = _size == null ? 255 : ((SizeAttribute)_size).Size;
            if (key != null)
                return $"`{c.Name}` {Methods.GetSqlType(c.PropertyType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            if (c.Name == _defaultId)
                return $"`{c.Name}` {Methods.GetSqlType(c.PropertyType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            else
                return $"`{c.Name}` {Methods.GetSqlType(c.PropertyType, size)} DEFAULT {Methods.GetSqlDefault(c.PropertyType)}";
        });
        var fields = typeof(T).GetFields().Select(c =>
        {
            var key = c.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var _size = c.GetCustomAttributes(typeof(SizeAttribute), false).FirstOrDefault();
            var size = _size == null ? 255 : ((SizeAttribute)_size).Size;
            if (key != null)
                return $"`{c.Name}` {Methods.GetSqlType(c.FieldType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            if (c.Name == _defaultId)
                return $"`{c.Name}` {Methods.GetSqlType(c.FieldType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            else
                return $"`{c.Name}` {Methods.GetSqlType(c.FieldType, size)} DEFAULT {Methods.GetSqlDefault(c.FieldType)}";
        });
        var all = properties.Concat(fields).ToList();
        await Db.Connection.ExecuteAsync($"CREATE TABLE IF NOT EXISTS `{table}` ({string.Join(", ", all)})");
        var result = await Db.Connection.ExecuteScalarAsync($"SHOW TABLES LIKE '{table}'");
        await Db.Connection.CloseAsync();
        return result != null;
    }
}

