using ECommerce.API.ActionFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ServiceFilter(typeof(BaseActionFilter))]
    public class BaseApiController : ControllerBase
    {
    }
}
