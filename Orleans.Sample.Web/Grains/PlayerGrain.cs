using Orleans.Streams;
using System.Threading.Tasks;

namespace Orleans.Sample.Web.Grains
{
    public class PlayerState
    {
        public string Name { get; set; }
    }
    
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task UpdatePlayerAsync(string name);
        
        Task<string> GetPlayerAsync();
    }
    
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private IAsyncStream<PlayerUpdated> _stream;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            
            _stream = GetStreamProvider("Stream")
                .GetStream<PlayerUpdated>(this.GetPrimaryKey(), nameof(PlayerUpdated));
        }

        public async Task UpdatePlayerAsync(string name)
        {
            State.Name = name;

            await WriteStateAsync();

            await _stream.OnNextAsync(new PlayerUpdated(name));
        }

        public Task<string> GetPlayerAsync()
        {
            return Task.FromResult(State.Name);
        }
    }
}