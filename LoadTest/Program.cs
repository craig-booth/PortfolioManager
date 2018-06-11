using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;


namespace LoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestCount = 100;

            var stockTest = new StockTest("http://localhost", new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D"));

            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            for (var i = 0; i < requestCount; i++)
            {
                var sendTask = stockTest.SendRequest();
                Task.WaitAll(sendTask);
            }
            stopWatch.Stop();

            Console.WriteLine("{0} requests in {1} millisecodes", requestCount, stopWatch.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }

    class StockTest
    {
        private HttpClient _HttpClient;

        public StockTest(string server, Guid apiKey)
        {
            _HttpClient = new HttpClient();
            _HttpClient.BaseAddress = new Uri(server);
            _HttpClient.DefaultRequestHeaders.Add("Api-Key", apiKey.ToString());
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> SendRequest()
        {
            HttpResponseMessage response = await _HttpClient.GetAsync("/api/stock/stocks");
            return response.IsSuccessStatusCode;
        }

    }
}
