using Microsoft.AspNetCore.SignalR;

namespace DrinkCatalog.Hubs
{
    public class NotificationHub : Hub
    {
        private static bool _isMachineOccupied = false;
        private static readonly List<string> _connectedUsers = new List<string>();
        public override async Task OnConnectedAsync()
        {
            _connectedUsers.Add(Context.ConnectionId);
            if (_isMachineOccupied)
            {
                await Clients.Caller.SendAsync("ReceiveNotification", "Извините, в данный момент автомат занят");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connectedUsers.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task NotifyClients(string message)
        {
            if (message == "MachineOccupied")
            {
                _isMachineOccupied = true;
                foreach (var connectionId in _connectedUsers)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", "Извините, в данный момент автомат занят");
                }
            }
            else if (message == "MachineFree")
            {
                _isMachineOccupied = false;
                foreach (var connectionId in _connectedUsers)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", "Автомат снова доступен");
                }
            }
        }
    }
}
