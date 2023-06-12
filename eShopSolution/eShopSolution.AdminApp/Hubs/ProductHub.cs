using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Hubs
{
    public class ProductHub : Hub
    {
        public async Task SendMessage(ProductVm productVm, IngredientAssignRequest request)
        {
            await Clients.All.SendAsync("ReceiveMessage", productVm, request);
        }
    }
}
