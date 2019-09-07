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
        protected readonly ClientSession _Session;
        private readonly JsonMediaTypeFormatter _Formatter;

        public RestResource(ClientSession session)
        {
            _Session = session;

            _Formatter = new JsonMediaTypeFormatter();
            _Formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _Formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _Formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            _Formatter.SerializerSettings.Converters.Add(new TransactionJsonConverter());
            _Formatter.SerializerSettings.Converters.Add(new CorporateActionJsonConverter());
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            T response = default(T);

            HttpResponseMessage httpResponse = await _Session.HttpClient.GetAsync(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { _Formatter });
            }

            return response;
        }

        protected async Task<bool> PostAsync(string url)
        {
            HttpResponseMessage response = await _Session.HttpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        protected async Task<bool> PostAsync<D>(string url, D data)
        {
            HttpResponseMessage response = await _Session.HttpClient.PostAsync<D>(url, data, _Formatter);
            return response.IsSuccessStatusCode;
        }

        protected async Task<T> PostAsync<T, D>(string url, D data)
        {
            T response = default(T);

            HttpResponseMessage httpResponse = await _Session.HttpClient.PostAsync<D>(url, data, _Formatter);
            if (httpResponse.IsSuccessStatusCode)
            {
                response = await httpResponse.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { _Formatter });
            }

            return response;
        }
    }
}
