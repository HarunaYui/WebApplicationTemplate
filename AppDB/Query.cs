using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MySqlConnector;
using WebApplicationTemplate.AppDB.Extensions;

namespace WebApplicationTemplate.AppDB;

/// <summary>
/// 封装数据库操作
/// </summary>
public static class Query
{
    /// <summary>
    /// 查询表全部数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cnn"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static async Task<List<T>> SqlSelect<T>(this MySqlConnection cnn, string table) where T : class
    {
        await cnn.OpenAsync();
        var result = (await cnn.QueryAsync<T>($"SELECT * FROM '{table}'")).ToList();
        await cnn.CloseAsync();
        return result;
    }

    /// <summary>
    /// 查询总数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cnn"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static async Task<int> SqlCountSelect<T>(this MySqlConnection cnn, string table) where T : class
    {
        await cnn.OpenAsync();
        var result = await cnn.ExecuteScalarAsync($"SELECT COUNT(*) FROM '{table}'");
        await cnn.CloseAsync();
        return Convert.ToInt32(result);
    }

    /// <summary>
    /// 检查表
    /// </summary>
    /// <param name="cnn"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static async Task<bool> CheckTable(this MySqlConnection cnn, string table)
    {
        await cnn.OpenAsync();
        var result = await cnn.ExecuteScalarAsync($"SHOW TABLES LIKE '{table}'");
        await cnn.CloseAsync();
        return result != null;
    }

    /// <summary>
    /// 创建表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cnn"></param>
    /// <param name="table"></param>
    /// <param name="defaultId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<bool> CreateTable<T>(this MySqlConnection cnn, string table = "", string defaultId = "ID") where T : class
    {
        if (string.IsNullOrEmpty(table))
        {
            var tableAttribute = typeof(T).GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            if (tableAttribute is not null)
                table = ((TableAttribute)tableAttribute).Name;
            else
                throw new ArgumentNullException(nameof(table));
        }
        if (await cnn.CheckTable(table))
            return true;
        await cnn.OpenAsync();
        var properties = typeof(T).GetProperties().Select(c =>
        {
            var key = c.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var _size = c.GetCustomAttributes(typeof(SizeAttribute), false).FirstOrDefault();
            var size = _size is null ? 255 : ((SizeAttribute)_size).Size;
            if (key != null)
                return $"`{c.Name}` {Methods.GetSqlType(c.PropertyType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            if (c.Name == defaultId)
                return $"`{c.Name}` {Methods.GetSqlType(c.PropertyType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            else
                return $"`{c.Name}` {Methods.GetSqlType(c.PropertyType, size)} DEFAULT {Methods.GetSqlDefault(c.PropertyType)}";
        });
        var fields = typeof(T).GetFields().Select(c =>
        {
            var key = c.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var _size = c.GetCustomAttributes(typeof(SizeAttribute), false).FirstOrDefault();
            var size = _size is null ? 255 : ((SizeAttribute)_size).Size;
            if (key != null)
                return $"`{c.Name}` {Methods.GetSqlType(c.FieldType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            if (c.Name == defaultId)
                return $"`{c.Name}` {Methods.GetSqlType(c.FieldType)} NOT NULL PRIMARY KEY AUTO_INCREMENT";
            else
                return $"`{c.Name}` {Methods.GetSqlType(c.FieldType, size)} DEFAULT {Methods.GetSqlDefault(c.FieldType)}";
        });
        var all = properties.Concat(fields).ToList();
        await cnn.ExecuteAsync($"CREATE TABLE IF NOT EXISTS `{table}` ({string.Join(", ", all)})");
        var result = await cnn.ExecuteScalarAsync($"SHOW TABLES LIKE '{table}'");
        await cnn.CloseAsync();
        return result != null;
    }
}

