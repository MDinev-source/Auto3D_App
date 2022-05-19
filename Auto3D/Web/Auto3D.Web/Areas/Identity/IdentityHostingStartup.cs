using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Auto3D.Web.Areas.Identity.IdentityHostingStartup))]

namespace Auto3D.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
