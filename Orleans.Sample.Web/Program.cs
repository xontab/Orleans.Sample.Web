using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Sample.Web.Grains;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Orleans.Sample.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseOrleans((ctx, x) =>
                {
                    x.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "players";
                    });
                    x.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = Dns
                        .GetHostEntry(Dns.GetHostName()).AddressList
                        .First(a => a.AddressFamily == AddressFamily.InterNetwork));
                    x.UseLocalhostClustering();
                    x.AddMemoryGrainStorageAsDefault();
                    x.UseConsulClustering(o =>
                        o.Address = new Uri(ctx.Configuration.GetValue<string>("Orleans:Consul:Address")));
                    x.ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences());
                });
    }
}