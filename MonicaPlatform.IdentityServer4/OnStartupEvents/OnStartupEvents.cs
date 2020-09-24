using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Monica.Core.Events;
using MonicaPlatform.IdentityServer4.Config;
using MonicaPlatform.IdentityServer4.Models;

namespace MonicaPlatform.IdentityServer4.OnStartupEvents
{
    /// <summary>
    /// События возникающие в классе Startup
    /// </summary>
    public class OnStartupEvents : IOnStartupEvents
    {

        public void OnConfigureBefore(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment, 
            ILoggerFactory loggerFactory)
        {
            Console.WriteLine(@"Platform.IdentityServer4.OnStartupEvents.OnStartupEvents.OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)");
            applicationBuilder.Use((context, next) =>
            {
                var head = context.Request.Headers;
                if (head.ContainsKey(@"Authorization")) return next();
                CookiesAuthorization(context);

                return next();
            });

            // Enable Authentication
            applicationBuilder.UseAuthentication();
            Console.WriteLine(@"app.UseAuthentication(); Platform.IdentityServer4.OnStartupEvents.OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)");
        }

        public void OnConfigureAfter(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory) { }

        /// <summary>
        /// После вызова services.AddMvc();
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="mvcBuilder">An interface for configuring MVC services.</param>
        /// <param name="configuration"></param>
        public void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder, IConfiguration configuration)
        {
            Console.WriteLine(@"Platform.IdentityServer4.OnStartupEvents.OnStartupEvents.OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder)");
            IdentityModelEventSource.ShowPII = true; //Add this line
            //Debugger.Launch();
            Configuration identityServer4Config;
            // Получаем конфигурацию
            if (!IdentityServer4ConfigLoader.LoadFromXml(out identityServer4Config, configuration))
            {
                identityServer4Config = new Configuration();
            }

            ConfigOptions configOptions = new ConfigOptions(identityServer4Config);

            // Пишем используемую конфигурацию
            Console.WriteLine(@"Используемая конфигурация:");
            Console.WriteLine(configOptions.ToString());

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler
                    {
                        // Disable the built-in JWT claims mapping feature.
                        InboundClaimTypeMap = new Dictionary<string, string>()
                    });
                    options.Authority = configOptions.Authority;
                    options.RequireHttpsMetadata = configOptions.RequireHttpsMetadata;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        // HACK включить проверку ValidateIssuer = true
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //ValidateLifetime = true,
                        //ValidateIssuerSigningKey = true,
                        //ValidIssuer = "myTenant.auth0.com",
                        //ValidAudience = "https://localhost:5001/api/v1",
                        //NameClaimType = "given_name"
                        NameClaimType = configOptions.NameClaimType,
                        RoleClaimType = configOptions.RoleClaimType,

                    };
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = authenticationFailedContext =>
                        {
                            authenticationFailedContext.Response.Clear();
                            authenticationFailedContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                    };
                });

            services.Configure<MvcOptions>(options =>
            {

                var policy = new AuthorizationPolicyBuilder();
                policy.AuthenticationSchemes.Clear();

                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                AuthorizationPolicy authorizationPolicy = policy.RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(authorizationPolicy));
            });


            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = string.Empty;
                options.AccessDeniedPath = "/AccessDenied";
                options.SlidingExpiration = true;
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.Clear();
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                };
            });

            Console.WriteLine(@"added filter Platform.IdentityServer4.OnStartupEvents.OnStartupEvents.OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder)");
        }

        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services)
        {
        }

        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder) { }

        public void OnInitBackendService(IServiceCollection services) { }

        /// <summary>
        /// После app.UseMvc();
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Represents a type used to configure the logging system and create instances of ILogger from the registered ILoggerProviders.</param>
        /// <param name="iLoggerFactory">Represents a type used to perform logging.</param>
        public void OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
        }

        public void OnConfigureAfterUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory) { }

        /// <summary>
        /// Cookies авторизация
        /// </summary>
        /// <param name="context"></param>
        private static void CookiesAuthorization(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                string bearer = context.Request.Cookies["jwt"];
                if (!string.IsNullOrWhiteSpace(bearer) && bearer.StartsWith(@"Bearer "))
                    context.Request.Headers.Add(@"Authorization", bearer);
            }
            else if (context.Request.Headers.TryGetValue(@"Cookie", out StringValues stringValues))
            {
                bool find = false;
                foreach (var cooks in from cookies in stringValues where !string.IsNullOrWhiteSpace(cookies) select cookies.Trim().Split(";", StringSplitOptions.RemoveEmptyEntries))
                {
                    foreach (string cook in cooks)
                    {
                        if (string.IsNullOrWhiteSpace(cook)) continue;
                        string c = cook.Trim();
                        if (!c.StartsWith(@"Bearer ")) continue;
                        find = true;
                        string bearer = c.Substring(4, c.Length - 4);
                        context.Request.Headers.Add(@"Authorization", bearer);
                        break;
                    }

                    if (find) break;
                }
            }
        }
    }
}
