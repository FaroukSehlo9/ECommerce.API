using ECommerce.API.Extentions;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicBroom.APIServices.ActionFilter
{
    public class BaseActionFilter : IAsyncActionFilter
    {
        private readonly IUserService _userService;

        public BaseActionFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1️⃣ Get UserId from token
            string userIdString = context.HttpContext.GetUserId();

            if (string.IsNullOrEmpty(userIdString))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "Login required"
                });
                return;
            }

            // 2️⃣ Validate Guid safely
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "Invalid token"
                });
                return;
            }

            // 3️⃣ Get user from database
            var user = _userService.GetUserById(userId);

            if (user == null)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    message = "User not found"
                });
                return;
            }

            // 4️⃣ Role check (3 = Customer مثلاً)
            if (user.Role == 3)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    message = "NoPermission"
                });
                return;
            }

            // 5️⃣ Continue
            await next();
        }
    }
}