using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiGateWay
{
    public class DefinedAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var contentBuilder = new StringBuilder();
          
            var userResponse = responses[0].Items["DownstreamResponse"] as DownstreamResponse;
            var userAuditLogResponse = responses[1].Items["DownstreamResponse"] as DownstreamResponse;
           
            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userAuditJson = await userAuditLogResponse.Content.ReadAsStringAsync();
         
            contentBuilder.Append(userJson);
            contentBuilder.Append(userAuditJson);
           

            var stringContent = new StringContent(contentBuilder.ToString())
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            return await Task.FromResult(new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK"));
        }
    }
}