using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Monica.Core.Utils;
using MonicaPlatform.IdentityServer4.Models;

namespace MonicaPlatform.IdentityServer4.Config
{
    /// <summary>
    /// Загрузчик конфигурации из XML
    /// </summary>
    internal static class IdentityServer4ConfigLoader
    {

        /// <summary>
        /// Читаем из XML конфигурацию
        /// </summary>
        public static bool LoadFromXml(out Configuration identityServer4Config, IConfiguration configuration)
        {
            bool.TryParse(configuration["IdentityServer4:Options:RequireHttpsMetadata"], out var isReq);
            identityServer4Config = new Configuration();
            identityServer4Config.Options = new ConfigurationOptions();
            identityServer4Config.Options.ApiName = configuration["IdentityServer4:Options:ApiName"];
            identityServer4Config.Options.Authority = configuration["IdentityServer4:Options:Authority"];
            identityServer4Config.Options.RequireHttpsMetadata = isReq;
            identityServer4Config.Options.Claims = new ConfigurationOptionsClaims();
            identityServer4Config.Options.Claims.NameClaimType = configuration["IdentityServer4:Options:Claims:NameClaimType"];
            identityServer4Config.Options.Claims.RoleClaimType = configuration["IdentityServer4:Options:Claims:RoleClaimType"];
            return true;
        }
    }
}
