using Sitecore.Modules.EmailCampaign;

namespace Sitecore.Support.Modules.EmailCampaign.Core.HostnameMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.Links;
    using Sitecore.Modules.EmailCampaign.Core.HostnameMapping;

    public class HostnameMappingService : Sitecore.Modules.EmailCampaign.Core.HostnameMapping.HostnameMappingService
    {
        private readonly ILogger _logger;

        private readonly IHostnameMappingRepository _hostnameMappingRepository;

        public HostnameMappingService(IHostnameMappingRepository hostnameMappingRepository, ILogger logger) : base(
            hostnameMappingRepository, logger)
        {
            _logger = logger;
            _hostnameMappingRepository = hostnameMappingRepository;
        }

        public override string GetServerUrl(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            UrlOptions options = new UrlOptions
            {
                AlwaysIncludeServerUrl = true
            };
            return new Uri(LinkManager.GetItemUrl(item, options), UriKind.Absolute).GetLeftPart(UriPartial.Authority);
        }

        protected override string GetMappedUrl(string originalUrl, HostnameMappingUrlType type, ManagerRoot managerRoot)
        {
            Assert.ArgumentNotNull(managerRoot, "managerRoot");
            if (string.IsNullOrEmpty(originalUrl))
            {
                _logger.LogWarn("Empty originalUrl supplied to HostnameMappingService.GetMappedUrl");
                return null;
            }
            Uri uri;
            try
            {
                uri = new Uri(originalUrl, UriKind.Absolute);
            }
            catch (Exception e)
            {
                _logger.LogError("Non-absolute originalUrl supplied to HostnameMappingService.GetMappedUrl: " + originalUrl, e);
                return originalUrl;
            }
            string leftPart = uri.GetLeftPart(UriPartial.Authority);
            HostnameMappingDefinition hostnameMappingDefinition = _hostnameMappingRepository.GetByHostname(leftPart);
            if (hostnameMappingDefinition == null)
            {
                if (!string.Equals(leftPart, GlobalSettings.RendererUrl, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("No mapping definition found for hostname: " + leftPart);
                    return originalUrl;
                }
                _logger.LogDebug("Defining mapping definition from RendererUrl and ManagerRoot for hostname: " + leftPart);
                hostnameMappingDefinition = new HostnameMappingDefinition
                {
                    Original = leftPart,
                    Preview = managerRoot.Settings.PreviewBaseURL,
                    Public = managerRoot.Settings.BaseURL
                };
            }
            switch (type)
            {
                case HostnameMappingUrlType.Preview:
                    return (hostnameMappingDefinition.Preview ?? hostnameMappingDefinition.Public) + uri.PathAndQuery;
                case HostnameMappingUrlType.Public:
                    return hostnameMappingDefinition.Public + uri.PathAndQuery;
                default:
                    return originalUrl;
            }
        }
    }
}
