using System;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
using Square;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    internal sealed class SquareSandboxTestConfig
    {
        public string AccessToken { get; set; }
        public string BaseUrl { get; set; } = SquareEnvironment.Sandbox;

        public static SquareSandboxTestConfig Load()
        {
            var repoRoot = FindRepoRoot();
            var localSettings = Path.Combine(repoRoot, "squareservice.pipeline.fourth.com", "appsettings.Local.json");
            var config = File.Exists(localSettings)
                ? JsonSerializer.Deserialize<SettingsRoot>(File.ReadAllText(localSettings), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.SquareSandbox
                : null;

            var accessToken = Environment.GetEnvironmentVariable("FOURTH_PIPELINE_SANDBOX_SQUARE_ACCESS_TOKEN")
                ?? config?.AccessToken;
            var baseUrl = Environment.GetEnvironmentVariable("FOURTH_PIPELINE_SANDBOX_SQUARE_BASE_URL")
                ?? config?.BaseUrl
                ?? SquareEnvironment.Sandbox;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Assert.Ignore("Set FOURTH_PIPELINE_SANDBOX_SQUARE_ACCESS_TOKEN or squareservice.pipeline.fourth.com/appsettings.Local.json SquareSandbox:AccessToken.");
            }

            return new SquareSandboxTestConfig
            {
                AccessToken = accessToken,
                BaseUrl = baseUrl
            };
        }

        private static string FindRepoRoot()
        {
            var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "service.pipeline.fourth.com.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not find repository root.");
        }

        private sealed class SettingsRoot
        {
            public SquareSandboxTestConfig SquareSandbox { get; set; }
        }
    }
}
