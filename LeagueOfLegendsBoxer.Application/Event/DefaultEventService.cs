using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reactive.Linq;
using Websocket.Client;

namespace LeagueOfLegendsBoxer.Application.Event
{
    public class DefaultEventService : IEventService
    {
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
                        Credentials = new NetworkCredential("riot", token),
                        RemoteCertificateValidationCallback =
                            (sender, cert, chain, sslPolicyErrors) => true,
                    }
                };

                socket.Options.AddSubProtocol("wamp");
                return socket;
            });

            _webSocket
                .MessageReceived
                .Where(msg => msg.Text != null)
                .Where(msg => msg.Text.StartsWith('['))
                .Subscribe(msg =>
                {
                    // Check if the message is json received from the client
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
            await _webSocket?.Start();
            await _webSocket?.SendInstant("[5, \"OnJsonApiEvent\"]");
        }

        public Task<bool> DisconnectAsync()
        {
            throw new NotImplementedException();
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
