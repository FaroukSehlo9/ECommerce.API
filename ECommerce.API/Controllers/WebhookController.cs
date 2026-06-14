using ECommerce.Application.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IEnumerable<IWebhookHandler> _handlers;

        public WebhookController(IEnumerable<IWebhookHandler> handlers)
        {
            _handlers = handlers;
        }

        [HttpPost("{provider}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Handle(string provider, [FromBody] object payload) // أضفنا payload
        {
            // دلوقت الـ Swagger هيشوف [FromBody] وهيظهر خانة الـ Request body فوراً

            // بما إننا استخدمنا [FromBody]، الـ payload هنا هيكون جاهز، 
            // بس عشان إحنا محتاجين الـ Raw string، هنستخدم الـ StreamReader برضه
            // أو ببساطة نحول الـ object لـ string:
            string json = payload?.ToString();

            var headers = Request.Headers.ToDictionary(k => k.Key, v => v.Value.ToString());

            var handler = _handlers.FirstOrDefault(h =>
                h.ProviderName.Equals(provider, StringComparison.OrdinalIgnoreCase));

            if (handler == null)
                return NotFound(new { message = $"Provider '{provider}' not supported" });

            var result = await handler.HandleAsync(json, headers);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
