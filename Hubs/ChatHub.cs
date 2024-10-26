using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace healthycannab.Hubs
{
    public class ChatHub : Hub
    {
        // Método para enviar mensajes a una sala específica
        public async Task SendMessage(string user, string message, string room)
        {
            await Clients.Group(room).SendAsync("ReceiveMessage", user, message);
        }

        // Método para unirse a una sala
        public async Task JoinRoom(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
        }

        // Método para salir de una sala
        public async Task LeaveRoom(string room)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        }
    }
}
