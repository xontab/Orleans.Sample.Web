using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Orleans.Sample.Web.Grains
{
    public interface IAuditGrain : IGrainWithGuidKey
    {
    }

    [ImplicitStreamSubscription(nameof(PlayerUpdated))]
    public class AuditGrain : Grain, IAuditGrain, IAsyncObserver<PlayerUpdated>
    {
        private readonly ILogger<AuditGrain> _logger;
        private StreamSubscriptionHandle<PlayerUpdated> _handler;

        public AuditGrain(ILogger<AuditGrain> logger)
        {
            _logger = logger;
        }
        
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var stream = GetStreamProvider("Stream")
                .GetStream<PlayerUpdated>(this.GetPrimaryKey(), nameof(PlayerUpdated));
            _handler = await stream.SubscribeAsync(this);
        }

        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();

            await _handler.UnsubscribeAsync();
        }

        public Task OnNextAsync(PlayerUpdated item, StreamSequenceToken token = null)
        {
            _logger.LogInformation("Player {name} updated at {date}", item.Name, DateTime.UtcNow);
            
            DeactivateOnIdle();
            
            return Task.CompletedTask;
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}