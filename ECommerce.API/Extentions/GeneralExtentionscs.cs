namespace ECommerce.API.Extentions
{
    public static class GeneralExtentions
    {

        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null || httpContext.User.Claims.Count() == 0)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(x => x.Type == "user_id").Value;
        }


        public static int? GetRole(this HttpContext httpContext)
        {
            if (httpContext.User == null || httpContext.User.Claims.Count() == 0)
            {
                return null;
            }

            return Convert.ToInt32(httpContext.User.Claims.Single(x => x.Type == "role_id").Value);
        }
        public static string GetCustomerId(this HttpContext httpContext)
        {
            if (httpContext.User == null || httpContext.User.Claims.Count() == 0)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(x => x.Type == "CustomerId").Value;
        }
        
    }
}
