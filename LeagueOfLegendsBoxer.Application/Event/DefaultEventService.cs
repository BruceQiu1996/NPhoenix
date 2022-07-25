using Newtonsoft.Json.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Security.Authentication;
using WebSocketSharp;

namespace LeagueOfLegendsBoxer.Application.Event
{
    public class DefaultEventService : IEventService
    {
        private const int ClientEventData = 2;
        private const int ClientEventNumber = 8;
        private WebSocket _webSocket;
        private readonly IDictionary<string, List<EventHandler<EventArgument>>> _subscribers = new Dictionary<string, List<EventHandler<EventArgument>>>();

        public EventHandler<EventArgument> MessageReceived { get; set; }
        public EventHandler<string> ErrorReceived { get; set; }

        public Task Initialize(int port, string token)
        {
            _webSocket = new WebSocket($"wss://127.0.0.1:{port}/", "wamp");
            _webSocket.SetCredentials("riot", token, true);
            _webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            _webSocket.SslConfiguration.ServerCertificateValidationCallback = (response, cert, chain, errors) => true;

            _webSocket.OnMessage += WssOnOnMessage;

            return Task.CompletedTask;
        }

        private void WssOnOnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText) return;

            var eventArray = JArray.Parse(e.Data);
            var eventNumber = eventArray[0].ToObject<int>();
            if (eventNumber != ClientEventNumber) return;
            var leagueEvent = eventArray[ClientEventData].ToObject<EventArgument>();
            if (string.IsNullOrWhiteSpace(leagueEvent?.Uri))
                return;

            MessageReceived?.Invoke(this, leagueEvent);
            if (!_subscribers.TryGetValue(leagueEvent.Uri, out List<EventHandler<EventArgument>> eventHandlers))
            {
                return;
            }

            eventHandlers.ForEach(eventHandler => eventHandler?.Invoke(this, leagueEvent));
        }

        public Task ConnectAsync()
        {
            _webSocket?.Connect();
            _webSocket?.Send("[5, \"OnJsonApiEvent\"]");

            return Task.CompletedTask;
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
