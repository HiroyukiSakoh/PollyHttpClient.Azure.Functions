using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using PollyHttpClient.Azure.WebJobs.Extensions.Bindings;
using System;
using System.Net.Http;

namespace PollyHttpClient.Azure.WebJobs.Extensions.Config
{
    [Extension("PollyHttpClient")]
    internal class PollyHttpClientExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly IHttpClientFactory httpClientFactory;

        public PollyHttpClientExtensionConfigProvider(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // HttpClientFactory Bindings
            var bindingAttributeBindingRule = context.AddBindingRule<PollyHttpClientAttribute>();
            bindingAttributeBindingRule.BindToInput<HttpClient>((httpClientFactoryAttribute) =>
            {
                return httpClientFactory.CreateClient(nameof(PollyHttpClientExtensionConfigProvider));
            });
        }
    }
}
