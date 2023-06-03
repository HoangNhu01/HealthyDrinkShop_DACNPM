using eShopSolution.ViewModels.Common;

namespace eShopSolution.AdminApp.IpAddresss
{
    public interface IIpAdrress
    {
        ApiResult<string> GetLocalIPAddress();
    }
}
