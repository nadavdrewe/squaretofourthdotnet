using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using domain.pipeline.fourth.com.Models;
using Microsoft.EntityFrameworkCore;

namespace web.pipeline.fourth.com.Services
{
    public class ClientAccessService
    {
        private readonly FourthPipelineContext _context;
        public ClientAccessService(FourthPipelineContext context) => _context = context;

        public bool IsPlatformAdmin(ClaimsPrincipal user) => user?.IsInRole("Administrator") == true;

        public async Task<bool> CanAccessBrandAsync(ClaimsPrincipal user, int brandId)
        {
            if (IsPlatformAdmin(user)) return true;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrWhiteSpace(userId) && await _context.ClientAccesses.AnyAsync(x => x.BrandId == brandId && x.UserId == userId && x.Active);
        }

        public async Task<IQueryable<data.pipeline.fourth.com.Models.Brand>> AccessibleBrandsAsync(ClaimsPrincipal user)
        {
            if (IsPlatformAdmin(user)) return _context.Brands;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.Brands.Where(x => userId != null && x.ClientAccesses.Any(a => a.UserId == userId && a.Active));
        }
    }
}
