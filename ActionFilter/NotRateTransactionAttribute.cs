namespace WebApplicationTemplate.ActionFilter;

/// <summary>
/// 不需要访问限制特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NotRateTransactionAttribute : Attribute
{
}

