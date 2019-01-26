using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PollyHttpClient.Azure.WebJobs.Extensions.Bindings;

namespace SampleTimerFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run(
            [TimerTrigger("0 */5 * * * *")]TimerInfo myTimer
            , [PollyHttpClient]HttpClient httpClient
            , ILogger log)
        {
            log.LogInformation("start func");
            await SendRequest(httpClient, log, "https://httpstat.us/200");
            await SendRequest(httpClient, log, "https://httpstat.us/500");
            await SendRequest(httpClient, log, "https://httpstat.us/429");
            //await SendRequest(httpClient, log, "https://httpstat.us/200?sleep=11000");
            log.LogInformation("finish func");
        }

        private static async Task SendRequest(HttpClient httpClient, ILogger log, string url)
        {
            try
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                }

            }
            catch (HttpRequestException ex)
            {
                log.LogError(ex, "HttpRequestException");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Exception");
            }
        }
    }
}
