using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Transactions;

namespace WebApplicationTemplate.ActionFilter;

/// <summary>
/// 事件回退事务
/// </summary>
public class TransactionScopeFilter : IAsyncActionFilter
{
    /// <summary>
    /// 事务
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //context.ActionArguments是当前被执行Action方法的描述信息
        var ctrlActionDesc = context.ActionDescriptor as ControllerActionDescriptor;
        bool isTX = false;
        if (ctrlActionDesc != null)
        {
            //ctrlActionDesc.MethodInfo 是当前Action方法
            bool hasNotTransactionalAttribute = ctrlActionDesc.MethodInfo.GetCustomAttributes(typeof(NotTransactionAttribute), false).Any();
            isTX = !hasNotTransactionalAttribute;
        }

        if (isTX)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var r = await next();
                if (r.Exception == null)
                {
                    tx.Complete();
                }
            }
        }
        else
        {
            await next();
        }
    }
}

