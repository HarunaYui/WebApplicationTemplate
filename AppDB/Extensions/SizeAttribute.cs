namespace WebApplicationTemplate.AppDB.Extensions;

/// <summary>
/// 数据库大小特性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SizeAttribute : Attribute
{
    /// <summary>
    /// 大小
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="size"></param>
    public SizeAttribute(int size)
    {
        Size = size;
    }
}

