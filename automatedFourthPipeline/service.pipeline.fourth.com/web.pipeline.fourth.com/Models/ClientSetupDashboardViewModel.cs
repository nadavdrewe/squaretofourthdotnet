using System;
using System.Collections.Generic;

namespace web.pipeline.fourth.com.Models
{
    public sealed class ClientSetupDashboardViewModel
    {
        public IReadOnlyList<ClientSetupClientViewModel> Clients { get; set; }
    }

    public sealed class ClientSetupClientViewModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool Active { get; set; }
        public int StoreCount { get; set; }
        public int IntegrationCount { get; set; }
        public IReadOnlyList<ClientSetupSquareConnectionViewModel> SquareConnections { get; set; }
    }

    public sealed class ClientSetupSquareConnectionViewModel
    {
        public int CredentialId { get; set; }
        public string Environment { get; set; }
        public bool Active { get; set; }
        public string MerchantId { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string ExpiresAt { get; set; }
    }
}