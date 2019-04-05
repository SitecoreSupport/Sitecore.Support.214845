using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Modules.EmailCampaign.Core.HostnameMapping;
using HostnameMappingService = Sitecore.Support.Modules.EmailCampaign.Core.HostnameMapping.HostnameMappingService;

namespace Sitecore.Support.Modules.EmailCampaign.DependencyInjection
{
  internal class CustomServiceConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      serviceCollection.AddTransient<IHostnameMappingService, HostnameMappingService>();
    }
  }
}