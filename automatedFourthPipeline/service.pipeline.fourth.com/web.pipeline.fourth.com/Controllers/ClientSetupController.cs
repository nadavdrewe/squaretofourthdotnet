using System.Linq;
using System.Threading.Tasks;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class ClientSetupController : Controller
    {
        private readonly FourthPipelineContext _context;
        private readonly web.pipeline.fourth.com.Services.ClientAccessService _access;

        public ClientSetupController(FourthPipelineContext context, web.pipeline.fourth.com.Services.ClientAccessService access)
        {
            _context = context;
            _access = access;
        }

        public async Task<IActionResult> Index(string oauth, int? brandId)
        {
            var brands = await (await _access.AccessibleBrandsAsync(User))
                .Include(x => x.Stores)
                .Include(x => x.BrandIntegrations)
                .Include(x => x.BrandCredentials)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var model = new ClientSetupDashboardViewModel
            {
                IsPlatformAdmin = _access.IsPlatformAdmin(User),
                Clients = brands.Select(brand => new ClientSetupClientViewModel
                {
                    BrandId = brand.Id,
                    BrandName = brand.Name,
                    Active = brand.Active,
                    StoreCount = brand.Stores.Count,
                    IntegrationCount = brand.BrandIntegrations.Count(x => x.Active),
                    SquareConnections = brand.BrandCredentials
                        .Where(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
                        .OrderByDescending(x => x.Active)
                        .ThenByDescending(x => x.WhenUpdatedUTC)
                        .Select(x =>
                        {
                            var metadata = SquareOAuthTokenMetadata.FromStoredValue(x.SupplimentalData2);
                            return new ClientSetupSquareConnectionViewModel
                            {
                                CredentialId = x.Id,
                                Environment = metadata?.GetEnvironment(x.BaseEndpoint) ?? SquareOAuthEnvironment.GetEnvironmentFromBaseUrl(x.BaseEndpoint),
                                Active = x.Active,
                                MerchantId = metadata?.SquareMerchantId,
                                LastUpdatedUtc = x.WhenUpdatedUTC,
                                ExpiresAt = metadata?.SquareAccessTokenExpiresAt
                            };
                        })
                        .ToList()
                }).ToList()
            };

            ViewData["OAuthStatus"] = oauth == "connected" && brandId.HasValue
                ? "Square connection saved and verified."
                : null;
            return View(model);
        }
    }
}
