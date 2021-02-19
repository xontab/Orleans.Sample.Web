using Dynatrace.OneAgent.Sdk.Api;
using Dynatrace.OneAgent.Sdk.Api.Enums;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Orleans.Sample.Web.Filters
{
    public class TraceGrainCallFilter : IOutgoingGrainCallFilter, IIncomingGrainCallFilter
    {
        private readonly IOneAgentSdk _oneAgentSdk;

        public TraceGrainCallFilter()
        {
            _oneAgentSdk = OneAgentSdkFactory.CreateInstance();
        }
        
        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            var tracer = _oneAgentSdk.TraceOutgoingMessage(
                _oneAgentSdk.CreateMessagingSystemInfo("Orleans", context.Grain.ToString(), MessageDestinationType.QUEUE, ChannelType.IN_PROCESS,
                    ""));

            if (RequestContext.Get("TraceId") != null)
            {
                tracer.SetCorrelationId(RequestContext.Get("TraceId").ToString());
            }

            await tracer.TraceAsync(async () => { await context.Invoke(); });
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            var tracer = _oneAgentSdk.TraceIncomingMessageProcess(
                _oneAgentSdk.CreateMessagingSystemInfo("Orleans", context.Grain.ToString(), MessageDestinationType.QUEUE, ChannelType.IN_PROCESS,
                    ""));

            if (RequestContext.Get("TraceId") != null)
            {
                tracer.SetCorrelationId(RequestContext.Get("TraceId").ToString());
            }

            await tracer.TraceAsync(async () => { await context.Invoke(); });
        }
    }
}