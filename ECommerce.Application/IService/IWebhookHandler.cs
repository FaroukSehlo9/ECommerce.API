using ECommerce.Application.Communications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IWebhookHandler
    {
        string ProviderName { get; }
        Task<GeneralResponse<bool>> HandleAsync(string payload, IDictionary<string, string> headers);
    }
}
