using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using PollyHttpClient.Azure.WebJobs.Extensions;
using PollyHttpClient.Azure.WebJobs.Extensions.Config;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(PollyHttpClientWebJobsStartup))]

namespace PollyHttpClient.Azure.WebJobs.Extensions
{
    public class PollyHttpClientWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<PollyHttpClientExtensionConfigProvider>();
            builder.Services.AddHttpClient<PollyHttpClientExtensionConfigProvider>(nameof(PollyHttpClientExtensionConfigProvider))
            .SetHandlerLifetime(System.Threading.Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(() =>
#if DEBUG
                 new HttpClientHandler()
                 {
                     Proxy = new System.Net.WebProxy("http://localhost:8888"),//Fiddler
                     UseProxy = true,
                 }
#else
                new HttpClientHandler()
                {
                }
#endif
            )
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || (int)msg.StatusCode == 429)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));

            //ASP.NET Core 2.2の対応
            //https://github.com/tpeczek/HttpClientFactory.Azure.Functions/commit/d9f5f788d468a6a3ff90414926a1c4d078f02976
            builder.Services.Configure<HttpClientFactoryOptions>(nameof(PollyHttpClientExtensionConfigProvider), options => options.SuppressHandlerScope = true);

            //HttpClientFactoryの標準ロガーがApplicationInsightsのLiveMetricStreamでノイジーなので、ロガーから無効化する
            //https://www.stevejgordon.co.uk/httpclientfactory-asp-net-core-logging
            //builder.Services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, SilentLoggingFilter>());
        }
    }

    public class SilentLoggingFilter : IHttpMessageHandlerBuilderFilter
    {
        public SilentLoggingFilter(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            return next;
        }
    }
}
