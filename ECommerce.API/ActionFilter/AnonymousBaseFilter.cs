using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.API.ActionFilter
{
    public class AnonymousBaseFilter : IAsyncActionFilter
    {


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {


            await next();

        }



    }
}
