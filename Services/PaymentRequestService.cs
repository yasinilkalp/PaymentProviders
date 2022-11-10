using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentProviders.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PaymentProviders.Services
{
    public interface IPaymentRequestService
    {
        Task<T> SendRequest<T>(string contentObject);
    }

    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly HttpClient _client;
        public PaymentRequestService(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> SendRequest<T>(string contentObject)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpRequestMessage httpRequest = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress.AbsoluteUri)
            };

            if (!string.IsNullOrEmpty(contentObject))
            {
                httpRequest.Content = new StringContent(contentObject, Encoding.UTF8, "text/xml");
            }

            HttpResponseMessage response = await _client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var successlog = await response.Content.ReadAsStringAsync();
                XmlSerializer serializer = new (typeof(T));
                using (TextReader reader = new StringReader(successlog))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }

            var errorlog = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<T>(errorlog);

            return errorResponse;
        }

    }
}
