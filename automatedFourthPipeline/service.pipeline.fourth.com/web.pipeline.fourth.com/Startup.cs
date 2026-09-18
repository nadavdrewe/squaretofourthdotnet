using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using web.pipeline.fourth.com.Data;
using web.pipeline.fourth.com.Models;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FourthPipelineContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FourthSalesPipelineContext")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddOptions<PublicOnboardingOptions>()
                .Bind(Configuration.GetSection("PublicOnboarding"))
                .ValidateOnStart();
            services.AddOptions<SapAssistantOptions>()
                .Bind(Configuration.GetSection("SapAssistant"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Access/Login";
                    options.AccessDeniedPath = "/Access/Login";
                    options.Cookie.Name = "SquareToFourth.Access";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddOptions<StaticAdminOptions>()
                .Bind(Configuration.GetSection("StaticAdmin"))
                .Validate(options => !string.IsNullOrWhiteSpace(options.Username),
                    "StaticAdmin:Username must be configured.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.PasswordHash) &&
                                     options.PasswordHash.Length == 64 &&
                                     options.PasswordHash.All(Uri.IsHexDigit),
                    "StaticAdmin:PasswordHash must be a SHA-256 hash represented as 64 hexadecimal characters.")
                .ValidateOnStart();

            var dataProtection = services.AddDataProtection().SetApplicationName("SquareToFourth");
            var keysDirectory = Configuration["DataProtection:KeysDirectory"];
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var isDevelopment = string.Equals(environmentName, Environments.Development, StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(keysDirectory))
            {
                if (!isDevelopment)
                {
                    throw new InvalidOperationException(
                        "DataProtection:KeysDirectory must be configured outside development so admin sessions survive restarts.");
                }
            }
            else
            {
                Directory.CreateDirectory(keysDirectory);
                dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keysDirectory));
            }

            if (Configuration.GetValue<bool>("ForwardedHeaders:Enabled"))
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.KnownIPNetworks.Clear();
                    options.KnownProxies.Clear();
                });
            }

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddMemoryCache();
            services.Configure<SquareOAuthOptions>(Configuration.GetSection("SquareOAuth"));
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("square-oauth", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
                options.AddPolicy("site-login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
                options.AddPolicy("public-onboarding", context =>
                    RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(10), QueueLimit = 0, AutoReplenishment = true }));
                options.AddPolicy("sap-assistant", context =>
                    RateLimitPartition.GetTokenBucketLimiter(
                        (context.Request.RouteValues["id"]?.ToString() ?? "unknown") + ":" + (context.Connection.RemoteIpAddress?.ToString() ?? "unknown"),
                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 8,
                            TokensPerPeriod = 4,
                            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
            });

            services.AddScoped<SquareOAuthTokenService>();
            services.AddScoped<SquareCredentialService>();
            services.AddScoped<SquareOAuthConfigurationService>();
            services.AddScoped<ClientAccessService>();
            services.AddScoped<OnboardingEmailSender>();
            services.AddScoped<SapDiscoveryStore>();
            services.AddHttpClient<SapAssistantService>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(45);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (Configuration.GetValue<bool>("ForwardedHeaders:Enabled"))
            {
                app.UseForwardedHeaders();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;
                var allowsAnonymous = context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null;
                var isPublicEndpoint = allowsAnonymous ||
                    path.StartsWithSegments("/Access") ||
                    path.StartsWithSegments("/oauthredirect/accept") ||
                    path.StartsWithSegments("/Onboarding") ||
                    path.StartsWithSegments("/health");
                if (!isPublicEndpoint && context.User.Identity?.IsAuthenticated != true)
                {
                    await context.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }

                await next();
            });
            app.Use(async (context, next) =>
            {
                if (context.User.Identity?.IsAuthenticated == true && !context.User.IsInRole("Administrator"))
                {
                    var path = context.Request.Path;
                    var isClientEndpoint = path == "/" ||
                        path.StartsWithSegments("/sap") ||
                        path.StartsWithSegments("/Home") ||
                        path.StartsWithSegments("/ClientSetup") ||
                        path.StartsWithSegments("/OauthLogin") ||
                        path.StartsWithSegments("/oauthredirect") ||
                        path.StartsWithSegments("/Access") ||
                        path.StartsWithSegments("/HowTo") ||
                        path.StartsWithSegments("/health");
                    if (!isClientEndpoint)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("This account does not have platform administration access.");
                        return;
                    }
                }
                await next();
            });
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
