namespace WebApplicationTemplate.AppDB.Extensions;

[AttributeUsage(AttributeTargets.Property)]
public class SizeAttribute : Attribute
{
    public int Size { get; set; }

    public SizeAttribute(int size)
    {
        Size = size;
    }
}

