using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable StringLiteralTypo
namespace LabTest
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            await WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(collection =>
                {
                    collection.AddHsts(options =>
                        {
                            options.Preload = true;
                            options.IncludeSubDomains = true;
                            options.MaxAge = TimeSpan.FromDays(365);
                        })
                        .AddHttpsRedirection(options =>
                        {
                            options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                            options.HttpsPort = 443;
                        })
                        .Configure<CookiePolicyOptions>(options =>
                        {
                            options.Secure = CookieSecurePolicy.Always;
                            options.CheckConsentNeeded = context => true;
                            options.MinimumSameSitePolicy = SameSiteMode.Strict;
                        })
                        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.LoginPath = new PathString("/Home/Login");
                            options.Cookie.Name = "labtest.admin";
                        });
                    collection.AddAntiforgery(options =>
                        {
                            options.Cookie.Name = "x.csrf.labtest";
                            options.HeaderName = "x-csrf-token";
                        })
                        .Configure<GzipCompressionProviderOptions>(options =>
                            options.Level = CompressionLevel.Optimal)
                        .AddResponseCompression(options =>
                        {
                            options.EnableForHttps = true;
                            options.Providers.Add<GzipCompressionProvider>();
                            options.MimeTypes = new List<string> {"*/*"};
                        })
                        .AddRouting(options => options.LowercaseUrls = true)
                        .AddMvc(option => option.EnableEndpointRouting = false)
                        .AddRazorRuntimeCompilation().SetCompatibilityVersion(CompatibilityVersion.Latest);
                })
                .Configure(builder =>
                {
                    builder.UseHsts().UseHttpsRedirection().UseCookiePolicy().UseAuthentication()
                        .UseResponseCompression().UseStaticFiles().UseResponseCaching()
                        .UseStatusCodePagesWithRedirects("/").UseMvcWithDefaultRoute();
                })
                .UseKestrel(c => c.AddServerHeader = false).Build().RunAsync();
        }
    }
}