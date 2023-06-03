using eShopSolution.ViewModels.Common;
using System.Net;

namespace eShopSolution.AdminApp.IpAddresss
{
    public class IpAddress : IIpAdrress
    {
        public ApiResult<string> GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            foreach (IPAddress address in addresses)
            {
                // Filter for IPv4 addresses
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return new ApiSuccessResult<string>(address.ToString());
                }
            }

            return new ApiErrorResult<string>(string.Empty);
        }
    }
}
