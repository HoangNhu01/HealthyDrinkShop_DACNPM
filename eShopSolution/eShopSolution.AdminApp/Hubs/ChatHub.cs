using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(ProductCreateRequest request)
        {
            await Clients.All.SendAsync("ReceiveMessage", request);
        }
        
    }
}