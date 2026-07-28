using Microsoft.Data.SqlClient;
using RevelFourthPipeline.Infrastructure.Configuration;

namespace RevelFourthPipeline.Tests;

public class LegacyBluebirdIntegrationSourceTests
{
    [Fact]
    public void TrustServerCertificate_ForcesTrustServerCertificateTrue()
    {
        var connectionString = LegacyBluebirdIntegrationSource.TrustServerCertificate(
            "Server=example;Database=revel;User ID=user;Password=pass;Encrypt=True");

        var builder = new SqlConnectionStringBuilder(connectionString);

        Assert.True(builder.TrustServerCertificate);
        Assert.True(builder.Encrypt);
        Assert.Equal("example", builder.DataSource);
        Assert.Equal("revel", builder.InitialCatalog);
    }
}
