using Microsoft.AspNetCore.Mvc;
using Orleans.Sample.Web.Grains;
using System;
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
            return await _clusterClient.GetGrain<IPlayerGrain>(id).GetPlayerAsync();
        }  
        
        [HttpPost]
        public async Task<Guid> Post(string name)
        {
            var id = Guid.NewGuid();
            
            await _clusterClient.GetGrain<IPlayerGrain>(id).UpdatePlayerAsync(name);
            
            return id;
        }  
    }
}