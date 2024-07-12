using DnsBman.Data;
using DnsBman.Components;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using DnsBman.Services;
using DnsBman.Utilities;
using Microsoft.AspNetCore.Identity;
using DnsBman.Models.IdentityModels;
using DnsBman.Services.ApiKey;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace DnsBman
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var arubaToken = await ArubaTokenHandler.GetToken();

                var builder = WebApplication.CreateBuilder(args);

                // Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Error()
                    .WriteTo.MSSqlServer(
                        connectionString: builder.Configuration.GetConnectionString("DnsBmanDB"), // locale
                        //connectionString: ConfigurationHandler.GetConnectionString(),
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true }
                    ).CreateLogger();

                // Authorization
                builder.Services.AddAuthorizationBuilder();

                // Database context
                builder.Services.AddDbContext<DnsBmanContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DnsBmanDB"))); // locale
                    //options.UseSqlServer(ConfigurationHandler.GetConnectionString()));

                // Identity
                builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<DnsBmanContext>()
                    .AddDefaultTokenProviders();

                // Identity configuration
                builder.Services.Configure<IdentityOptions>(options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                    options.User.RequireUniqueEmail = true;
                });

                builder.Services.ConfigureApplicationCookie(options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/";
                    options.SlidingExpiration = true;
                });

                // CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(name: "MyPolicy", configurePolicy: policyBuilder =>
                    {
                        policyBuilder.WithOrigins("https://sviluppo.bman.it", "https://cloud.bman.it", "https://beta.bman.it")
                                     .WithHeaders("X-API-KEY")
                                     .AllowAnyMethod();
                    });
                });

                // Controller
                builder.Services.AddControllers();

                builder.Services.AddQuickGridEntityFrameworkAdapter();

                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                // Http client
                builder.Services.AddHttpClient("ArubaApi", async client =>
                {
                    if (ArubaTokenHandler.IsExpiring(arubaToken))
                    {
                        arubaToken = await ArubaTokenHandler.GetToken();
                    }

                    client.BaseAddress = new Uri("https://api.arubabusiness.it/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", arubaToken.AccessToken);
                    client.DefaultRequestHeaders.Add("Authorization-Key", ConfigurationHandler.GetArubaAuthKey());
                });

                // Services
                builder.Services.AddScoped<DomainService>();
                builder.Services.AddScoped<RecordClientService>();
                builder.Services.AddScoped<BmanCustomerService>();

                // ApiKey service
                builder.Services.AddScoped<ApiKeyAuthFilter>();
                builder.Services.AddScoped<ApiKeyHandler>();
                builder.Services.AddTransient<IApiKeyValidation, ApiKeyValidation>();

                // HttpContext
                builder.Services.AddHttpContextAccessor();

                builder.Services.AddCascadingAuthenticationState();

                builder.Services.AddAntiforgery();

                var app = builder.Build();

                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();

                app.UseCors("MyPolicy");

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
                
                app.UseAntiforgery();
                
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Errore durante l'esecuzione dell'applicazione");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
