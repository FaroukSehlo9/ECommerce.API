using ECommerce.API.Extentions;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.API.ActionFilter
{
    public class BaseActionFilter : IAsyncActionFilter
    {

        private readonly IUserService _UserService;
        public BaseActionFilter(IUserService UserService)
        {
            _UserService = UserService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            string UserName = context.HttpContext.GetUserId();//token


            if (string.IsNullOrEmpty(UserName))
            {
                context.Result = new RedirectToRouteResult(
                   new RouteValueDictionary(new { controller = "Message", action = "LoginFirst" }));
            }
            else
            {
                Guid userId = Guid.Parse(context.HttpContext.GetUserId());
                var User = _UserService.GetUserById(userId);
                if (User == null)
                {
                    context.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Message", action = "UserNotFound" }));
                }

                else if (User.Role == (long)RoleType.Customer)
                {
                    context.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Message", action = "NoPermission" }));
                }
                else
                {
                    await next();
                }
            }

        }



    }
}
