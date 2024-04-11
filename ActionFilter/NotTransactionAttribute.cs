namespace WebApplicationTemplate.ActionFilter;

/// <summary>
/// 不需要回退事务特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NotTransactionAttribute : Attribute
{

}

