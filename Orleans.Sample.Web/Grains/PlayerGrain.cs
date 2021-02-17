using System.Threading.Tasks;

namespace Orleans.Sample.Web.Grains
{
    public class PlayerState
    {
        public string Name { get; set; }
    }
    
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task CreatePlayerAsync(string name);
        
        Task<string> GetPlayerAsync();
    }
    
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        public async Task CreatePlayerAsync(string name)
        {
            await Task.Delay(100);

            State.Name = name;
        }

        public Task<string> GetPlayerAsync()
        {
            return Task.FromResult(State.Name);
        }
    }
}