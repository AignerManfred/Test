using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Http;

using Sapl.Pdp.Api;

using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Sapl.Pdp.Remote
{
    public class RemotePolicyDecisionPoint : IPolicyDecisionPoint
    {
        public class RemotePolicyDecisionPointConnection : IDisposable
        {
            private const string DefaultEventType = "message";

            public delegate void MessageReceivedHandler(object sender, string data);
            public delegate void DisconnectEventHandler(object sender);

            private HttpClient httpClient;
            private Stream Stream = null;
            private readonly Uri uri;

            private HttpContent httpContent;

            private volatile bool _IsDisposed = false;
            public bool IsDisposed => _IsDisposed;

            private volatile bool IsReading = false;
            private readonly object StartLock = new object();

            private int ReconnectDelay = 3000;
            private string LastEventId = string.Empty;

            public event MessageReceivedHandler MessageReceived;
            public event DisconnectEventHandler Disconnected;

            /// <summary>
            /// An instance of EventSourceReader
            /// </summary>
            /// <param name="url">URL to listen from</param>
            /// <param name="handler">An optional custom handler for HttpClient</param>
            public RemotePolicyDecisionPointConnection(Uri uri, HttpContent httpContent, HttpMessageHandler httpMessageHandler)
            {
                this.uri = uri;
                this.httpClient = new HttpClient(httpMessageHandler);

                this.httpContent = httpContent;

                var creds = string.Format("{0}:{1}", "YJidgyT2mfdkbmL", "Fa4zvYQdiwHZVXh");
                var basicAuth = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(creds)));
                httpClient.DefaultRequestHeaders.Add("Authorization", basicAuth);
            }


            /// <summary>
            /// Returns instantly and starts listening
            /// </summary>
            /// <returns>current instance</returns>
            /// <exception cref="ObjectDisposedException">Dispose() has been called</exception>
            public RemotePolicyDecisionPointConnection Start()
            {
                if (_IsDisposed)
                {
                    throw new ObjectDisposedException("EventSourceReader");
                }
                lock (StartLock)
                {
                    if (IsReading == false)
                    {
                        IsReading = true;
                        // Only start a new one if one isn't already running
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        ReaderAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                }
                return this;
            }


            /// <summary>
            /// Stop and dispose of the EventSourceReader
            /// </summary>
            public void Dispose()
            {
                _IsDisposed = true;
                Stream?.Dispose();
                httpClient.CancelPendingRequests();
                httpClient.Dispose();
            }


            private async Task ReaderAsync()
            {
                try
                {
                    if (string.Empty != LastEventId)
                    {
                        if (httpClient.DefaultRequestHeaders.Contains("Last-Event-Id"))
                        {
                            httpClient.DefaultRequestHeaders.Remove("Last-Event-Id");
                        }

                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Last-Event-Id", LastEventId);
                    }


                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, this.uri);
                    httpRequestMessage.Content = httpContent;

                    using (HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        if (response.Headers.TryGetValues("content-type", out IEnumerable<string> ctypes) || ctypes?.Contains("text/event-stream") == false)
                        {
                            throw new ArgumentException("Specified URI does not return server-sent events");
                        }

                        Stream = await response.Content.ReadAsStreamAsync();
                        using (var sr = new StreamReader(Stream))
                        {
                            string evt = DefaultEventType;
                            string id = string.Empty;

                            var data = new StringBuilder(string.Empty);

                            while (true)
                            {
                                string line = await sr.ReadLineAsync();
                                if (line == string.Empty)
                                {
                                    // double newline, dispatch message and reset for next
                                    if (data.Length > 0)
                                    {
                                        // TODO: xtr evtName, evtId, MessageReceived?.Invoke(this, new EventSourceMessageEventArgs(data.ToString().Trim(), evt, id));
                                        MessageReceived?.Invoke(this, data.ToString().Trim());
                                    }
                                    data.Clear();
                                    id = string.Empty;
                                    evt = DefaultEventType;
                                    continue;
                                }
                                else if (line.First() == ':')
                                {
                                    // Ignore comments
                                    continue;
                                }

                                int dataIndex = line.IndexOf(':');
                                string field;
                                if (dataIndex == -1)
                                {
                                    dataIndex = line.Length;
                                    field = line;
                                }
                                else
                                {
                                    field = line.Substring(0, dataIndex);
                                    dataIndex += 1;
                                }

                                string value = line.Substring(dataIndex).Trim();

                                switch (field)
                                {
                                    case "event":
                                        // Set event type
                                        evt = value;
                                        break;
                                    case "data":
                                        // Append a line to data using a single \n as EOL
                                        data.Append($"{value}\n");
                                        break;
                                    case "retry":
                                        // Set reconnect delay for next disconnect
                                        int.TryParse(value, out ReconnectDelay);
                                        break;
                                    case "id":
                                        // Set ID
                                        LastEventId = value;
                                        id = LastEventId;
                                        break;
                                    default:
                                        // Ignore other fields
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Disconnect(ex);
                }
            }

            private void Disconnect(Exception ex)
            {
                //Console.WriteLine(ex.ToString());

                IsReading = false;
                Disconnected?.Invoke(this);
            }
        }


        private static readonly string DECIDE = "/api/pdp/decide";

        private static readonly string MULTI_DECIDE = "/api/pdp/multi-decide";

        private static readonly string MULTI_DECIDE_ALL = "/api/pdp/multi-decide-all";

        private Uri baseUri;

        private HttpMessageHandler httpMessageHandler;


        private int firstBackoffMillis { get; set; } = 500;

        private int maxBackOffMillis { get; set; } = 5000;

        private int backoffFactor { get; set; } = 2;


        //TODO: xtr
        //public RemotePolicyDecisionPoint(string baseUrl, string clientKey, string clientSecret /* TODO: xtr , SslContext sslContext */)
        //{
        //	this(baseUrl, clientKey, clientSecret, HttpClient.create().secure(spec->spec.sslContext(sslContext)));
        //}

        public RemotePolicyDecisionPoint(Uri baseUri, string clientKey, string clientSecret, HttpMessageHandler httpMessageHandler = null)
        {
            this.baseUri = baseUri;

            this.httpMessageHandler = httpMessageHandler ?? new HttpClientHandler();
        }

        //TODO: xtr
        //public RemotePolicyDecisionPoint(String baseUrl, String clientKey, String clientSecret, HttpClient httpClient)
        //{
        //	client = WebClient.builder().clientConnector(new ReactorClientHttpConnector(httpClient)).baseUrl(baseUrl)
        //			.defaultHeaders(header->header.setBasicAuth(clientKey, clientSecret)).build();
        //}

        //TODO: xtr
        //private Repeat<?> repeat()
        //{
        //	return Repeat.onlyIf(repeatContext-> true)
        //			.backoff(Backoff.exponential(Duration.ofMillis(firstBackoffMillis), Duration.ofMillis(maxBackOffMillis),
        //					backoffFactor, false))
        //			.doOnRepeat(o->log.debug("No connection to remote PDP. Reconnect: {}", o));
        //}

        public IPolicyDecisionPointStreamControl decide(AuthorizationSubscription authzSubscription)
        {
            //            var type = new ParameterizedTypeReference<ServerSentEvent<AuthorizationDecision>>()  { };
            //            return decide(DECIDE, type, authzSubscription)
            //                    .onErrorResume(__->Flux.just(AuthorizationDecision.INDETERMINATE)).repeatWhen(repeat())
            //                    .distinctUntilChanged();

            //var httpContent = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(authzSubscription));
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(authzSubscription)));
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            Exchangeable<AuthorizationDecision> exchangeable = authzSubscription.Exchangeable;

            var evt = new RemotePolicyDecisionPointConnection(new Uri(this.baseUri, DECIDE), httpContent, httpMessageHandler).Start();

            evt.MessageReceived += (object sender, string data) =>
            {
                Console.WriteLine("event xxx: " + data);
                //exchangeable.Value = JsonSerializer.Deserialize<AuthorizationDecision>(data);
                exchangeable.Value = JsonConvert.DeserializeObject<AuthorizationDecision>(data);
            };

            evt.Disconnected += async (object sender) =>
            {
                if (exchangeable.Value.Decision != Decision.INDETERMINATE)
                {
                    exchangeable.Value = new AuthorizationDecision(Decision.INDETERMINATE);
                }

                Console.WriteLine($"Retry: {100}");
                await Task.Delay(100);
                evt.Start(); // Reconnect to the same URL
            };

            return null;
        }

  
        public IPolicyDecisionPointStreamControl decide(MultiAuthorizationSubscription multiAuthzSubscription)
        {
            //            var type = new ParameterizedTypeReference<ServerSentEvent<AuthorizationDecision>>()  { };
            //            return decide(DECIDE, type, authzSubscription)
            //                    .onErrorResume(__->Flux.just(AuthorizationDecision.INDETERMINATE)).repeatWhen(repeat())
            //                    .distinctUntilChanged();

            //var httpContent = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(authzSubscription));
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(multiAuthzSubscription)));
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Exchangeable<AuthorizationDecision> exchangeable = new Exchangeable<AuthorizationDecision>(new AuthorizationDecision(Decision.INDETERMINATE));

            var evt = new RemotePolicyDecisionPointConnection(new Uri(this.baseUri, MULTI_DECIDE), httpContent, httpMessageHandler).Start();

            var authorizationExchangeables = multiAuthzSubscription.AuthorizationExchangeables;

            evt.MessageReceived += (object sender, string data) =>
            {
                Console.WriteLine("event xxx: " + data);

                IdentifiableAuthorizationDecision identifiableAuthorizationDecision = JsonConvert.DeserializeObject<IdentifiableAuthorizationDecision>(data);

                Exchangeable<AuthorizationDecision> exchangeable = null;
                authorizationExchangeables.TryGetValue(identifiableAuthorizationDecision.AuthorizationSubscriptionId, out exchangeable);

                if (exchangeable != null)
                {
                    System.Console.WriteLine(identifiableAuthorizationDecision.AuthorizationDecision.ToString());
                    exchangeable.Value = identifiableAuthorizationDecision.AuthorizationDecision;
                }

            };

            evt.Disconnected += async (object sender) =>
            {
                //if (exchangeable.Value.decision != Decision.INDETERMINATE)
                //{
                //    exchangeable.Value = new AuthorizationDecision(Decision.INDETERMINATE);
                //}

                foreach (var item in authorizationExchangeables)
                {
                    if (item.Value.Value.Decision != Decision.INDETERMINATE)
                    {
                        item.Value.Value = new AuthorizationDecision(Decision.INDETERMINATE);
                    }
                }

                Console.WriteLine($"Retry: {100}");
                await Task.Delay(100);
                evt.Start(); // Reconnect to the same URL
            };

            return null;
        }

        //public Exchangeable<MultiAuthorizationDecision> decideAll(MultiAuthorizationSubscription multiAuthzSubscription)
        //{
            //	var type = new ParameterizedTypeReference<ServerSentEvent<MultiAuthorizationDecision>>()
            //	{
            //	};
            //	return decide(MULTI_DECIDE_ALL, type, multiAuthzSubscription)
            //			.onErrorResume(__->Flux.just(MultiAuthorizationDecision.indeterminate())).repeatWhen(repeat())
            //			.distinctUntilChanged();
        //    return null;
        //}

        //private < T > Flux<T> decide(String path, ParameterizedTypeReference<ServerSentEvent<T>> type,
        //		Object authzSubscription) {
        //	return client.post().uri(path).accept(MediaType.APPLICATION_NDJSON).contentType(MediaType.APPLICATION_JSON)
        //			.bodyValue(authzSubscription).retrieve().bodyToFlux(type).mapNotNull(ServerSentEvent::data)
        //			.doOnError(error->log.error("Error : {}", error.getMessage()));
        //}
    }
}



