using System;
using System.Linq;
using Sitecore.Diagnostics;
using Sitecore.EmailCampaign.Model.Web.Settings;
using Sitecore.ExM.Framework.Diagnostics;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Core.HostnameMapping;
using Sitecore.Web;

namespace Sitecore.Support.Modules.EmailCampaign.Core.HostnameMapping
{
  public class HostnameMappingService : Sitecore.Modules.EmailCampaign.Core.HostnameMapping.HostnameMappingService
  {
    private readonly IHostnameMappingRepository _hostnameMappingRepository;
    private readonly ILogger _logger;

    public HostnameMappingService(IHostnameMappingRepository hostnameMappingRepository, ILogger logger) : base(
      hostnameMappingRepository, logger)
    {
      Assert.ArgumentNotNull(hostnameMappingRepository, "hostnameMappingRepository");
      Assert.ArgumentNotNull(logger, "logger");

      _hostnameMappingRepository = hostnameMappingRepository;
      _logger = logger;
    }

    protected override string GetMappedUrl(string originalUrl, HostnameMappingUrlType type, ManagerRoot managerRoot)
    {
      Assert.ArgumentNotNull(managerRoot, "managerRoot");

      Uri uri;
      if (string.IsNullOrEmpty(originalUrl))
      {
        _logger.LogWarn("Empty originalUrl supplied to HostnameMappingService.GetMappedUrl");
        return null;
      }

      try
      {
        uri = new Uri(originalUrl, UriKind.Absolute);
      }
      catch (Exception exception)
      {
        _logger.LogError("Non-absolute originalUrl supplied to HostnameMappingService.GetMappedUrl: " + originalUrl,
          exception);
        return originalUrl;
      }

      var leftPart = uri.GetLeftPart(UriPartial.Authority);
      var byHostname = _hostnameMappingRepository.GetByHostname(leftPart);
      if (byHostname == null)
      {
        if (!IsInternalLink(originalUrl) &&
            !string.Equals(leftPart, GlobalSettings.RendererUrl, StringComparison.OrdinalIgnoreCase))
        {
          _logger.LogDebug("No mapping definition found for hostname: " + leftPart);
          return originalUrl;
        }

        byHostname = new HostnameMappingDefinition
        {
          Original = leftPart,
          Preview = managerRoot.Settings.PreviewBaseURL,
          Public = managerRoot.Settings.BaseURL
        };
      }

      if (type != HostnameMappingUrlType.Preview)
      {
        if (type == HostnameMappingUrlType.Public)
        {
          return byHostname.Public + uri.PathAndQuery;
        }

        return originalUrl;
      }

      return (byHostname.Preview ?? byHostname.Public) + uri.PathAndQuery;
    }

    private bool IsInternalLink(string url)
    {
      Uri uri;
      if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
      {
        return false;
      }

      var hostname = uri.GetLeftPart(UriPartial.Authority);

      var serverUrl = WebUtil.GetServerUrl();
      var knownHostnames = GetKnownHostnames(serverUrl);
      return knownHostnames.Contains(hostname, StringComparer.InvariantCultureIgnoreCase);
    }
  }
}