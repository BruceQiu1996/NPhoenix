using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Reactive.Linq;
using Websocket.Client;

namespace LeagueOfLegendsBoxer.Application.Event
{
    public class DefaultEventService : IEventService
    {
        private bool _loopAlive = false;
        private const int ClientEventData = 2;
        private const int ClientEventNumber = 8;
        private WebsocketClient _webSocket;
        private readonly IDictionary<string, List<EventHandler<EventArgument>>> _subscribers = new Dictionary<string, List<EventHandler<EventArgument>>>();

        public EventHandler<EventArgument> MessageReceived { get; set; }
        public EventHandler<string> ErrorReceived { get; set; }

        public Task Initialize(int port, string token)
        {
            _webSocket = new WebsocketClient(new Uri($"wss://127.0.0.1:{port}/"), () =>
            {
                var socket = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                        Credentials = new NetworkCredential("riot", token),
                        RemoteCertificateValidationCallback =
                            (sender, cert, chain, sslPolicyErrors) => true,
                    }
                };
                socket.Options.AddSubProtocol("wamp");
                socket.Options.SetRequestHeader("Connection", "keep-alive");
                return socket;
            });

            _webSocket.DisconnectionHappened.Subscribe(async type =>
            {
                try
                {
                    await _webSocket?.Start();
                    await _webSocket?.SendInstant("[5, \"OnJsonApiEvent\"]");
                    SendMessage();
                }
                catch { }
            });

            _webSocket.ReconnectionHappened.Subscribe(async _ =>
            {
                try
                {
                    await _webSocket?.Start();
                    await _webSocket?.SendInstant("[5, \"OnJsonApiEvent\"]");
                    SendMessage();
                }
                catch { }
            });

            _webSocket.ErrorReconnectTimeout = TimeSpan.FromSeconds(3);
            _webSocket.ReconnectTimeout = TimeSpan.FromSeconds(3);
            _webSocket
                .MessageReceived
                .Where(msg => msg.Text != null)
                .Where(msg => msg.Text.StartsWith('['))
                .Subscribe(msg =>
                {
                    var eventArray = JArray.Parse(msg.Text);
                    var eventNumber = eventArray?[0].ToObject<int>();
                    if (eventNumber != ClientEventNumber)
                    {
                        return;
                    }

                    var leagueEvent = eventArray[ClientEventData].ToObject<EventArgument>();
                    MessageReceived?.Invoke(this, leagueEvent);
                    if (!_subscribers.TryGetValue(leagueEvent.Uri, out List<EventHandler<EventArgument>> eventHandlers))
                    {
                        return;
                    }

                    eventHandlers.ForEach(eventHandler => eventHandler?.Invoke(this, leagueEvent));
                });

            return Task.CompletedTask;
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _webSocket?.Start();
                await _webSocket?.SendInstant("[5, \"OnJsonApiEvent\"]");
                SendMessage();
            }
            catch (Exception ex) 
            {
            
            }
        }

        private void SendMessage() 
        {
            if (_loopAlive) return;
            _loopAlive = true;
            Task.Run(async () =>
            {
                while (true)
                {
                    await _webSocket?.SendInstant("[5, \"OnJsonApiEvent\"]");
                    await Task.Delay(2000);
                }
            });
        }

        public ValueTask<bool> DisconnectAsync()
        {
            _webSocket.Dispose();
            return new ValueTask<bool>(true);
        }

        public void Subscribe(string uri, EventHandler<EventArgument> eventHandler)
        {
            if (_subscribers.TryGetValue(uri, out var eventHandlers))
            {
                eventHandlers.Add(eventHandler);
            }
            else
            {
                _subscribers.Add(uri, new List<EventHandler<EventArgument>> { eventHandler });
            }
        }

        public bool Unsubscribe(string uri)
        {
            return _subscribers.Remove(uri);
        }

        public void UnsubscribeAll()
        {
            _subscribers.Clear();
        }
    }
}
