using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace TestWebApplication.ActionFilter
{
    public class RateLimitFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache memoryCache;
        public RateLimitFilter(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
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
                string removeIp = context.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                string cacheKey = $"LastVisitTick{removeIp}";
                long? lastTick = memoryCache.Get<long?>(cacheKey);
                if (lastTick == null || Environment.TickCount64 - lastTick > 1000)
                {
                    memoryCache.Set(cacheKey, Environment.TickCount64, TimeSpan.FromSeconds(10));
                    await next();
                }
                else
                {
                    ObjectResult result = new ObjectResult("访问频繁")
                    {
                        StatusCode = 429
                    };
                    context.Result = result;
                }
            }
            else
            {
                await next();
            }
        }
    }
}
