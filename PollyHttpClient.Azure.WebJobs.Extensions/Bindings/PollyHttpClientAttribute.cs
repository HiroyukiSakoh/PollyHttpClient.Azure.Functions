using Microsoft.Azure.WebJobs.Description;
using System;
using System.Net.Http;

namespace PollyHttpClient.Azure.WebJobs.Extensions.Bindings
{
    /// <summary>
    /// Attribute used to bind to a <see cref="HttpClient"/> instance.
    /// </summary>
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class PollyHttpClientAttribute : Attribute
    { }
}
