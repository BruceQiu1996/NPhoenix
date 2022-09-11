namespace LeagueOfLegendsBoxer.Application.Event
{
    public interface IEventService
    {
        EventHandler<EventArgument> MessageReceived { get; set; }

        EventHandler<string> ErrorReceived { get; set; }

        Task ConnectAsync();

        ValueTask<bool> DisconnectAsync();

        Task Initialize(int port, string token);

        void Subscribe(string uri, EventHandler<EventArgument> eventHandler);

        bool Unsubscribe(string uri);
        void UnsubscribeAll();
    }
}
