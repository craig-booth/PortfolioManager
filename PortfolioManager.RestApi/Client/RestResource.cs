using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using PortfolioManager.RestApi.Converters;

namespace PortfolioManager.RestApi.Client
{
    public abstract class RestResource
    {
        private readonly HttpClient _HttpClient;

        public RestResource(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            T response = default(T);

            HttpResponseMessage httpResponse = await _HttpClient.GetAsync(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                var formatter = new JsonMediaTypeFormatter();
                formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
                formatter.SerializerSettings.Converters.Add(new TransactionJsonConverter());
                formatter.SerializerSettings.Converters.Add(new CorporateActionJsonConverter());

                response = await httpResponse.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { formatter });
            }

            return response;
        }

        protected async Task<bool> PostAsync(string url)
        {
            HttpResponseMessage response = await _HttpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        protected async Task<bool> PostAsync<D>(string url, D data)
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            formatter.SerializerSettings.Converters.Add(new TransactionJsonConverter());
            formatter.SerializerSettings.Converters.Add(new CorporateActionJsonConverter());

            HttpResponseMessage response = await _HttpClient.PostAsync<D>(url, data, formatter);
            return response.IsSuccessStatusCode;
        }
    }
}
