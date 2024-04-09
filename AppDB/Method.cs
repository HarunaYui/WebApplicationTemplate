using System.Data;

namespace TestWebApplication.AppDB;
public static class Methods
{
    /// <summary>
    /// 默认表的主键名
    /// </summary>
    public static readonly string defaultId = "ID";
    /// <summary>
    /// 根据类型获取对应的 DbType
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static DbType TypeToDbType(Type type)
    {
        switch (type.Name)
        {
            case "String":
                return DbType.String;
            case "Int32":
                return DbType.Int32;
            case "Int64":
                return DbType.Int64;
            case "Single":
                return DbType.Single;
            case "Double":
                return DbType.Double;
            case "Decimal":
                return DbType.Decimal;
            case "Boolean":
                return DbType.Boolean;
            case "DateTime":
                return DbType.DateTime;
            case "Byte[]":
                return DbType.Binary;
            case "Guid":
                return DbType.Guid;
            default:
                {
                    if (type.IsEnum)
                        return DbType.Int32;
                    return DbType.String;
                }
        }
    }
    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Size"></param>
    /// <returns></returns>
    public static string GetSqlType(Type type, int Size)
    {
        switch (type.Name)
        {
            case "String":
                return $"VARCHAR({Size})";
            case "Int32":
                return "INT";
            case "Int64":
                return "BIGINT";
            case "Boolean":
                return "BOOLEAN";
            case "DateTime":
                return "DATETIME";
            case "Single":
                return "FLOAT";
            case "Double":
                return "DOUBLE";
            case "Byte[]":
                return "BLOB";
            case "Decimal":
                return "DECIMAL";
            case "Guid":
                return "CHAR(36)";
            default:
                {
                    if (type.IsEnum)
                        return "INT";
                    return $"VARCHAR({Size})";
                }
        }
    }
    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetSqlType(Type type)
    {
        return GetSqlType(type, 255);
    }
    /// <summary>
    /// 获取类型默认值
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetSqlDefault(Type type)
    {
        switch (type.Name)
        {
            case "String":
                return "''";
            case "Int32":
                return "0";
            case "Int64":
                return "0";
            case "Boolean":
                return "false";
            case "DateTime":
                return "'0001-01-01 00:00:00'";
            case "Single":
                return "0";
            case "Double":
                return "0";
            case "Byte[]":
                return "''";
            case "Decimal":
                return "0";
            case "Guid":
                return "''";
            default:
                {
                    if (type.IsEnum)
                        return "0";
                    return "''";
                }
        }
    }
}
