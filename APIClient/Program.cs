using System.Diagnostics;
using System.Net;

namespace APIClient
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            bool useHttpClient = true; // 变量，true 为使用 HttpClient，false 为使用 HttpWebRequest
            string url = "http://localhost:5043/WeatherForecast";

            int totalRequests = 1000; // 总请求数
            long totalElapsedTime = 0; // 总响应时间
            long fastestResponseTime = long.MaxValue; // 最快响应时间
            long slowestResponseTime = long.MinValue; // 最慢响应时间

            var httpClientHandler = new HttpClientHandler()
            {
                MaxConnectionsPerServer = 10, // 每个服务器的最大连接数
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, // 自动解压缩响应
                UseCookies = false, // 禁用 Cookie
                UseProxy = false, // 禁用代理
            };

            var httpClient = new HttpClient(httpClientHandler);


            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < totalRequests; i++)
            {
                long elapsedTime = 0;

                if (useHttpClient)
                {
                    stopwatch.Restart();
                    await CallApiWithHttpClient(url);
                    //await CallApiWithHttpClient2(httpClient, url);
                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                }
                else
                {
                    stopwatch.Restart();
                    await CallApiWithHttpWebRequest(url);
                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                }

                totalElapsedTime += elapsedTime;
                fastestResponseTime = Math.Min(fastestResponseTime, elapsedTime);
                slowestResponseTime = Math.Max(slowestResponseTime, elapsedTime);

                Console.WriteLine($"Request {i + 1}: Elapsed Time - {elapsedTime} ms");
            }

            double averageResponseTime = (double)totalElapsedTime / totalRequests;

            Console.WriteLine($"Total Requests: {totalRequests}");
            Console.WriteLine($"Fastest Response Time: {fastestResponseTime} ms");
            Console.WriteLine($"Slowest Response Time: {slowestResponseTime} ms");
            Console.WriteLine($"Average Response Time: {averageResponseTime} ms");
            Console.ReadKey();
        }

        static async Task CallApiWithHttpClient(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            // 可以在这里处理响应
            response.EnsureSuccessStatusCode();
        }

        static async Task CallApiWithHttpClient2(HttpClient httpClient, string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            // 可以在这里处理响应
            response.EnsureSuccessStatusCode();
        }

        static async Task CallApiWithHttpWebRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            // 可以在这里处理响应
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"API returned status code: {response.StatusCode}");
            }
        }
    }
}