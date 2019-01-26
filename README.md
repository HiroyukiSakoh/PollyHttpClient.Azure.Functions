# PollyHttpClient.Azure.Functions

* HttpClientにRetryを追加する為のWebJob Extention
* Retryには[Microsoft.Extensions.Http.Polly](https://docs.microsoft.com/ja-jp/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly)を使用する。

* Extentionの基本構成は[こちら](https://www.tpeczek.com/2018/12/alternative-approach-to-httpclient-in.html)を参考に実装している。


* 以下が解決した場合、不要となるハズ
  * [azure-functions-host issue#3737 Dependency Injection support for Functions](https://github.com/Azure/azure-functions-host/issues/3737)
  * [azure-functions-host issue#3736 Built in support for HttpClientFactory](https://github.com/Azure/azure-functions-host/issues/3736)
