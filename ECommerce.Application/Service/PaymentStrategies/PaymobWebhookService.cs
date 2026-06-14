using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class PaymobWebhookService : IWebhookHandler
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<GeneralMessages> _localization;

        public string ProviderName => "paymob";

        public PaymobWebhookService(
            IOrderService orderService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<GeneralMessages> localization)
        {
            _orderService = orderService;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _localization = localization;
        }

        public async Task<GeneralResponse<bool>> HandleAsync(string payload, IDictionary<string, string> headers)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(payload))
                {
                    return new GeneralResponse<bool>(false, "Payload is empty");
                }

                // 1. Get HMAC from Query Parameters
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return new GeneralResponse<bool>(false, "HTTP context is not available");
                }

                string? receivedHmac = httpContext.Request.Query["hmac"].ToString();
                if (string.IsNullOrEmpty(receivedHmac))
                {
                    return new GeneralResponse<bool>(false, "HMAC signature missing in query parameters");
                }

                // 2. Parse the payload
                var json = JObject.Parse(payload);
                var obj = json["obj"] as JObject;
                if (obj == null)
                {
                    return new GeneralResponse<bool>(false, "Invalid Paymob webhook payload: 'obj' is missing");
                }

                // 3. Extract and concatenate fields for HMAC calculation
                // Paymob fields must be concatenated in the exact lexicographical order of their keys
                string concatenated =
                    GetValue(obj["amount_cents"]) +
                    GetValue(obj["created_at"]) +
                    GetValue(obj["currency"]) +
                    GetValue(obj["error_occured"]) +
                    GetValue(obj["has_parent_transaction"]) +
                    GetValue(obj["id"]) +
                    GetValue(obj["integration_id"]) +
                    GetValue(obj["is_3d_secure"]) +
                    GetValue(obj["is_auth"]) +
                    GetValue(obj["is_capture"]) +
                    GetValue(obj["is_refunded"]) +
                    GetValue(obj["is_standalone_payment"]) +
                    GetValue(obj["pending"]) +
                    GetValue(obj["source_data"]?["pan"]) +
                    GetValue(obj["source_data"]?["sub_type"]) +
                    GetValue(obj["source_data"]?["type"]) +
                    GetValue(obj["success"]);

                // 4. Calculate SHA-512 HMAC
                var hmacSecret = _config["Paymob:HmacSecret"];
                if (string.IsNullOrEmpty(hmacSecret))
                {
                    return new GeneralResponse<bool>(false, "Paymob HMAC secret is missing in configuration");
                }

                string calculatedHmac = CalculateHmac(concatenated, hmacSecret);

                // 5. Verify signature
                if (!calculatedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase))
                {
                    return new GeneralResponse<bool>(false, "Invalid HMAC signature");
                }

                // 6. Process status and update database
                var paymobOrderId = obj["order"]?["id"]?.ToString() ?? obj["id"]?.ToString();
                if (string.IsNullOrEmpty(paymobOrderId))
                {
                    return new GeneralResponse<bool>(false, "Transaction or Order ID is missing in payload");
                }

                bool isSuccess = obj["success"]?.Value<bool>() == true && obj["pending"]?.Value<bool>() == false;
                string? errorMsg = obj["error_occured"]?.Value<bool>() == true ? "Transaction failed" : null;

                var confirmResult = await _orderService.ConfirmPaymentStatus(paymobOrderId, isSuccess, errorMsg);

                if (confirmResult.Success)
                {
                    return new GeneralResponse<bool>(true, _localization["Payment status updated successfully"].Value);
                }

                return new GeneralResponse<bool>(false, confirmResult.Message);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Paymob Webhook error: {ex.Message}");
            }
        }

        private string GetValue(JToken? token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return string.Empty;
            }

            if (token.Type == JTokenType.Boolean)
            {
                return token.Value<bool>().ToString().ToLower();
            }

            return token.ToString();
        }

        private string CalculateHmac(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
