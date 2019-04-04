using System;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Sitecore.EmailCampaign.Model.Web.Settings;
using Sitecore.EmailCampaign.XConnect.Web;
using Sitecore.ExM.Framework.Diagnostics;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Core;
using Sitecore.Modules.EmailCampaign.Core.Analytics;
using Sitecore.Modules.EmailCampaign.Core.Contacts;
using Sitecore.Modules.EmailCampaign.Core.Crypto;
using Sitecore.Modules.EmailCampaign.Core.Data;
using Sitecore.Modules.EmailCampaign.Core.Dispatch;
using Sitecore.Modules.EmailCampaign.Core.Gateways;
using Sitecore.Modules.EmailCampaign.Core.HostnameMapping;
using Sitecore.Modules.EmailCampaign.Core.MVTesting;
using Sitecore.Modules.EmailCampaign.Core.Pipelines.HandleMessageEventBase;
using Sitecore.Modules.EmailCampaign.Core.Services;
using Sitecore.Modules.EmailCampaign.Factories;
using Sitecore.Modules.EmailCampaign.ListManager;
using Sitecore.Modules.EmailCampaign.Messages;
using Sitecore.Modules.EmailCampaign.Messages.Interfaces;
using Sitecore.Modules.EmailCampaign.shell.EmailCampaign.Core.MVTesting;
using Sitecore.Modules.EmailCampaign.Services;

namespace Sitecore.Support.Modules.EmailCampaign.DependencyInjection
{
  internal class CustomServiceConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      serviceCollection.AddTransient<IXConnectClientFactory, XConnectClientFactory>();
      ServiceCollectionServiceExtensions.AddTransient<XConnectRetry>(serviceCollection, (Func<IServiceProvider, XConnectRetry>)(provider => (Factory.CreateObject("exm/xconnectRetry", true) as XConnectRetry)));
      serviceCollection.AddTransient<ItemUtilExt>();
      serviceCollection.AddSingleton<IExmCampaignService, ExmCampaignService>();
      serviceCollection.AddTransient<IManagerRootService, ManagerRootService>();
      ServiceCollectionServiceExtensions.AddSingleton<ILogger>(serviceCollection, (Func<IServiceProvider, ILogger>)(provider => (Factory.CreateObject("exmLogger", true) as ILogger)));
      ServiceCollectionServiceExtensions.AddTransient<IStringCipher>(serviceCollection, (Func<IServiceProvider, IStringCipher>)(provider => (Factory.CreateObject("exmAuthenticatedCipher", true) as IStringCipher)));
      ServiceCollectionServiceExtensions.AddTransient<IContactService>(serviceCollection, (Func<IServiceProvider, IContactService>)(provider => (Factory.CreateObject("exm/contactService", true) as IContactService)));
      ServiceCollectionServiceExtensions.AddTransient<AnalyticsFactory>(serviceCollection, (Func<IServiceProvider, AnalyticsFactory>)(provider => AnalyticsFactory.Instance));
      serviceCollection.AddTransient<IMessageStateInfoFactory, MessageStateInfoFactory>();
      serviceCollection.AddTransient<IRecipientManagerFactory, RecipientManagerFactory>();
      serviceCollection.AddTransient<ICultureProvider, CultureProvider>();
      serviceCollection.AddTransient<IHostnameMappingRepository, HostnameMappingRepository>();
      serviceCollection.AddTransient<IHostnameMappingService, Sitecore.Support.Modules.EmailCampaign.Core.HostnameMapping.HostnameMappingService>();
      serviceCollection.AddTransient<ILanguageRepository, LanguageRepository>();
      ServiceCollectionServiceExtensions.AddSingleton<ITypeResolverFactory>(serviceCollection, (Func<IServiceProvider, ITypeResolverFactory>)(provider => (Factory.CreateObject("exm/typeResolverFactory", true) as ITypeResolverFactory)));
      ServiceCollectionServiceExtensions.AddTransient<IMultiVariateTestStrategyFactory>(serviceCollection, (Func<IServiceProvider, IMultiVariateTestStrategyFactory>)(provider => (Factory.CreateObject("exm/multiVariateTestStrategyFactory", true) as IMultiVariateTestStrategyFactory)));
      ServiceCollectionServiceExtensions.AddTransient<IMessageStateInfoFactory>(serviceCollection, (Func<IServiceProvider, IMessageStateInfoFactory>)(provider => (Factory.CreateObject("exm/messageStateInfoFactory", true) as IMessageStateInfoFactory)));
      serviceCollection.AddTransient<IMessageItemSourceFactory, MessageItemSourceFactory>();
      serviceCollection.AddTransient<IMessageTestVariantRepository, MessageTestVariantRepository>();
      serviceCollection.AddTransient<IGuidCryptoServiceProviderFactory, GuidCryptoServiceProviderFactory>();
      serviceCollection.AddTransient<MVTestLabHelper>();
      serviceCollection.AddTransient<IMvTestCandidateManagerFactory, MvTestCandidateManagerFactory>();
      serviceCollection.AddTransient<IMvTestStatisticsFactory, MvTestStatisticsFactory>();
      serviceCollection.AddSingleton<IScheduleManager, ScheduleManager>();
      serviceCollection.AddTransient<MessageLanguageManager>();
      serviceCollection.AddTransient<Util>();
      serviceCollection.AddTransient<AnalyticsGateway, DefaultAnalyticsGateway>();
      ServiceCollectionServiceExtensions.AddTransient<EcmDataProvider>(serviceCollection, (Func<IServiceProvider, EcmDataProvider>)(provider => (Factory.CreateObject("exm/dataProvider", true) as EcmDataProvider)));
      serviceCollection.AddTransient<ListManagerWrapper>();
      serviceCollection.AddTransient<PipelineHelper>();
      serviceCollection.AddTransient<ModificationTracker>();
      ServiceCollectionServiceExtensions.AddTransient<QueryStringEncryption>(serviceCollection, (Func<IServiceProvider, QueryStringEncryption>)(provider => (Factory.CreateObject("queryStringEncryption", true) as QueryStringEncryption)));
      ServiceCollectionServiceExtensions.AddTransient<EcmSettings>(serviceCollection, (Func<IServiceProvider, EcmSettings>)(provider => ((Factory.CreateObject("ioFactory/settings", false) as EcmSettings) ?? GlobalSettings.Instance)));
      serviceCollection.AddTransient<DedicatedServersService>();
      serviceCollection.AddSingleton<MessageRelationsCorrector>();
      serviceCollection.AddSingleton<IAbnTestService, AbnTestService>();
      ServiceCollectionServiceExtensions.AddSingleton<IEventDataService>(serviceCollection, (Func<IServiceProvider, IEventDataService>)(provider => (Factory.CreateObject("exm/eventDataService", true) as IEventDataService)));
      ServiceCollectionServiceExtensions.AddSingleton<MessageBodyCache>(serviceCollection, (Func<IServiceProvider, MessageBodyCache>)(provider => new MessageBodyCache(GlobalSettings.MessageBodyMaxCacheSize)));
      serviceCollection.AddTransient<IPublishingTaskFactory, PublishingTaskFactory>();
    }

  }
}