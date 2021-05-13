using Dynatrace.OneAgent.Sdk.Api;
using Dynatrace.OneAgent.Sdk.Api.Enums;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace Orleans.Sample.Web.Filters
{

    class StdErrLoggingCallback : ILoggingCallback
    {
        public void Error(string message) => Console.Error.WriteLine("[OneAgent SDK] Error:   " + message);
        public void Warn(string message) => Console.Error.WriteLine("[OneAgent SDK] Warning: " + message);
    }
    public class TraceGrainCallFilter : IOutgoingGrainCallFilter, IIncomingGrainCallFilter
    {
        private readonly ILogger<TraceGrainCallFilter> _logger;
        private readonly IOneAgentSdk _oneAgentSdk;


        public TraceGrainCallFilter(ILogger<TraceGrainCallFilter> logger)
        {
            _logger = logger;
            _oneAgentSdk = OneAgentSdkFactory.CreateInstance();
            var loggingCallback = new StdErrLoggingCallback();
            _oneAgentSdk.SetLoggingCallback(loggingCallback);
        }

        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            var tracer = _oneAgentSdk.TraceOutgoingMessage(
                _oneAgentSdk.CreateMessagingSystemInfo("Orleans Outgoing", context?.Grain?.ToString(),
                    MessageDestinationType.QUEUE, ChannelType.IN_PROCESS,
                    ""));


            if (RequestContext.Get("TraceId") != null)
            {
                tracer.SetCorrelationId(RequestContext.Get("TraceId").ToString());

                _logger.LogDebug("Tracing Outgoing {grainType} Correlation Id {traceId}", context.Grain.GetType().FullName, RequestContext.Get("TraceId"));

                await tracer.TraceAsync(async () =>
                {
                    if (RequestContext.Get(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME) == null)
                    {
                        RequestContext.Set(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME, tracer.GetDynatraceByteTag());
                    }
                    // _logger.LogDebug("Outgoing Dyntrace Byte Tag  : {byte_tag}", RequestContext.Get(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME));
                    await context.Invoke();
                });
            }
            else
            {
                await context.Invoke();
            }
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            var tracer = _oneAgentSdk.TraceIncomingMessageProcess(
            _oneAgentSdk.CreateMessagingSystemInfo("Orleans Incoming", context?.Grain?.ToString(),
                MessageDestinationType.QUEUE, ChannelType.IN_PROCESS,
                ""));


            if (RequestContext.Get("TraceId") != null)
            {
                tracer.SetCorrelationId(RequestContext.Get("TraceId").ToString());
                tracer.SetDynatraceByteTag((byte[])RequestContext.Get(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME));
                //_logger.LogDebug("Incoming Dyntrace Byte Tag  : {byte_tag}", RequestContext.Get(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME));
                _logger.LogDebug("Tracing Incoming {grainType} Correlation Id {traceId}", context.Grain.GetType().FullName, RequestContext.Get("TraceId"));

                await tracer.TraceAsync(async () =>
                {
                    await context.Invoke();
                });
            }
            else
            {
                await context.Invoke();
            }
        }
    }
}