using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Monica.Core.Constants;
using Monica.Core.Utils;

namespace MonicaPlatform.AuthModule.Middleware
{
    class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var auth = AutoFac.ResolveNamed<IAuthUserEngine>(_configuration["AuthSchema"], true);
            if (auth != null)
                await auth.SetAuthUser(context);

            await _next(context);
        }
    }

    public interface IAuthUserEngine
    {
        Task SetAuthUser(HttpContext context);
    }

    public class AuthUserIs4 : IAuthUserEngine
    {
        public async Task SetAuthUser(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.User.Identity.Name))
            {
                if (!context.Items.ContainsKey(FilterConstant.UserName))
                {
                    context.Items[FilterConstant.UserName] = context.User.Identity.Name;
                }
            }
            else
            {
                var authResult = await context.AuthenticateAsync("Bearer");
                context.Items[FilterConstant.UserName] = authResult?.Principal?.Identity?.Name;
            }
        }
    }
}
