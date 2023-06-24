using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Hubs
{
    public class OrderHub : Hub
    {
        public async Task OrderCheckOut(CheckOutRequest request)
        {
            await Clients.All.SendAsync("ReceiveMessage", request);
        }
    }
}