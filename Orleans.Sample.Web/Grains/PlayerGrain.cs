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
        public async Task UpdatePlayerAsync(string name)
        {
            State.Name = name;

            await WriteStateAsync();
        }

        public Task<string> GetPlayerAsync()
        {
            return Task.FromResult(State.Name);
        }
    }
}