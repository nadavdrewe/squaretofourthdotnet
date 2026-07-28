using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Infrastructure.Abstractions;
using RevelFourthPipeline.Infrastructure.Configuration;
using RevelFourthPipeline.Infrastructure.Fourth;
using RevelFourthPipeline.Infrastructure.Mapping;
using RevelFourthPipeline.Infrastructure.Pipeline;
using RevelFourthPipeline.Infrastructure.Revel;

namespace RevelFourthPipeline.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRevelFourthPipeline(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RevelFourthPipelineOptions>()
            .Bind(configuration.GetSection(RevelFourthPipelineOptions.SectionName))
            .Validate(options => options.BusinessDayStartHour is >= 0 and <= 23, "BusinessDayStartHour must be between 0 and 23.")
            .Validate(options => !options.OverrideRangeStart.HasValue || !options.OverrideRangeEnd.HasValue || options.OverrideRangeStart.Value < options.OverrideRangeEnd.Value, "OverrideRangeStart must be earlier than OverrideRangeEnd.")
            .Validate(options => options.LegacyDatabase.Enabled || !string.IsNullOrWhiteSpace(options.Revel.BaseUrl), "Revel BaseUrl is required when the legacy database source is disabled.")
            .Validate(options => options.LegacyDatabase.Enabled || !options.Stores.Any(x => x.Active) || !string.IsNullOrWhiteSpace(options.Revel.ApiKeySecret), "Revel ApiKeySecret is required when active config stores are used.")
            .Validate(options => options.LegacyDatabase.Enabled || options.DryRun || !string.IsNullOrWhiteSpace(options.Fourth.Username), "Fourth Username is required when DryRun is false.")
            .Validate(options => options.LegacyDatabase.Enabled || options.DryRun || !string.IsNullOrWhiteSpace(options.Fourth.Password), "Fourth Password is required when DryRun is false.")
            .Validate(options => options.LegacyDatabase.Enabled || options.DryRun || !string.IsNullOrWhiteSpace(options.Fourth.OrganisationId), "Fourth OrganisationId is required when DryRun is false.")
            .Validate(options => options.LegacyDatabase.Enabled || options.DryRun || !string.IsNullOrWhiteSpace(options.Fourth.DefaultLocation) || options.Stores.All(x => !string.IsNullOrWhiteSpace(x.FourthLocation)), "Fourth location is required when DryRun is false.")
            .ValidateOnStart();

        services.AddHttpClient(RevelOperationsReportClient.HttpClientName)
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RevelFourthPipelineOptions>>()
                    .Value;

                client.Timeout = TimeSpan.FromSeconds(options.Revel.TimeoutSeconds);
            });

        services.AddHttpClient(RevelProductMixReportClient.HttpClientName)
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RevelFourthPipelineOptions>>()
                    .Value;

                client.Timeout = TimeSpan.FromSeconds(options.Revel.TimeoutSeconds);
            });

        services.AddHttpClient<IFourthSoapClient, FourthSoapClient>();

        services.AddSingleton<IRevelOperationsReportClient, RevelOperationsReportClient>();
        services.AddSingleton<IRevelProductMixReportClient, RevelProductMixReportClient>();
        services.AddSingleton<OptionsRevelFourthIntegrationSource>();
        services.AddSingleton<LegacyBluebirdIntegrationSource>();
        services.AddSingleton<IRevelFourthIntegrationSource>(provider =>
        {
            var options = provider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<RevelFourthPipelineOptions>>()
                .Value;

            return options.LegacyDatabase.Enabled
                ? provider.GetRequiredService<LegacyBluebirdIntegrationSource>()
                : provider.GetRequiredService<OptionsRevelFourthIntegrationSource>();
        });
        services.AddSingleton<IRevelOperationsToFourthMapper, RevelOperationsToFourthMapper>();
        services.AddSingleton<IRevelProductMixToFourthMapper, RevelProductMixToFourthMapper>();
        services.AddSingleton<IFourthSalesXmlBuilder, FourthSalesXmlBuilder>();
        services.AddSingleton<IFourthSubmissionLedger, FileFourthSubmissionLedger>();
        services.AddSingleton<IRevelFourthPipelineRunner, RevelFourthPipelineRunner>();

        return services;
    }
}
