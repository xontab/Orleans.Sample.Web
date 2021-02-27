using Microsoft.AspNetCore.Mvc;
using Orleans.Runtime;
using Orleans.Sample.Web.Grains;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orleans.Sample.Web.Controllers
{
    [ApiController]
    [Route("players")]
    public class PlayerController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public PlayerController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }
        
        [HttpGet("{id}")]
        public async Task<string> Get(Guid id)
        {
            RequestContext.Set("TraceId", Activity.Current.TraceId);

            return await _clusterClient.GetGrain<IPlayerGrain>(id).GetPlayerAsync();
        }  
        
        [HttpPost]
        public async Task<Guid> Post(string name)
        {
            var id = Guid.NewGuid();
            
            RequestContext.Set("TraceId", Activity.Current.TraceId);

            await _clusterClient.GetGrain<IPlayerGrain>(id).UpdatePlayerAsync(name);
            
            return id;
        }  
    }
}