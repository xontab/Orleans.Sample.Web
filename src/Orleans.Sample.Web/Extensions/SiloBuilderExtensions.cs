using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using System;

namespace Orleans.Sample.Web.Extensions
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder UseClustering(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("Orleans:Consul:IsEnabled"))
            {
                siloBuilder.UseConsulClustering(o =>
                    o.Address = new Uri(configuration.GetValue<string>("Orleans:Consul:Address")));
            }
            else
            {
                siloBuilder.UseLocalhostClustering();
            }

            return siloBuilder;
        }
    }
}