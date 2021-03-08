using Dynatrace.OneAgent.Sdk.Api;
using Dynatrace.OneAgent.Sdk.Api.Enums;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Orleans.Sample.Web.Filters
{
    public class TraceGrainCallFilter : IOutgoingGrainCallFilter, IIncomingGrainCallFilter
    {
        private readonly ILogger<TraceGrainCallFilter> _logger;
        private readonly IOneAgentSdk _oneAgentSdk;

        public TraceGrainCallFilter(ILogger<TraceGrainCallFilter> logger)
        {
            _logger = logger;
            _oneAgentSdk = OneAgentSdkFactory.CreateInstance();
        }

        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            var tracer = _oneAgentSdk.TraceOutgoingMessage(
                _oneAgentSdk.CreateMessagingSystemInfo("Orleans", context?.Grain?.ToString(),
                    MessageDestinationType.QUEUE, ChannelType.IN_PROCESS,
                    ""));

            if (RequestContext.Get("TraceId") != null)
            {
                tracer.SetCorrelationId(RequestContext.Get("TraceId").ToString());

                _logger.LogDebug("Tracing Outgoing {grainType} Correlation Id {traceId}", context.Grain.GetType().FullName, RequestContext.Get("TraceId"));

                await tracer.TraceAsync(async () => { await context.Invoke(); });
            }
            else
            {
                await context.Invoke();
            }
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            var tracer = _oneAgentSdk.TraceIncomingMessageProcess(
                _oneAgentSdk.CreateMessagingSystemInfo("Orleans", context?.Grain?.ToString(),
                    MessageDestinationType.QUEUE, ChannelType.IN_PROCESS,
                    ""));

            if (RequestContext.Get("TraceId") != null)
            {
                tracer.SetCorrelationId(RequestContext.Get("TraceId").ToString());
                
                _logger.LogDebug("Tracing Incoming {grainType} Correlation Id {traceId}", context.Grain.GetType().FullName, RequestContext.Get("TraceId"));

                await tracer.TraceAsync(async () => { await context.Invoke(); });
            }
            else
            {
                await context.Invoke();
            }
        }
    }
}