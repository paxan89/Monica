using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Monica.Core.Abstraction.Authorize;
using Monica.Core.DbModel.ModelsAuth;
using Monica.Core.Exceptions;

namespace Monica.Core.Service.Authorize
{
    /// <summary>
    /// Адаптер для авторизации пользователей в системе
    /// </summary>
    public class MonicaAuthorizeDataAdapter : IMonicaAuthorizeDataAdapter
    {
        private IConfiguration _configuration;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MonicaAuthorizeDataAdapter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Авторизация пользователя в системе. При успешном входе пользователю будет отправлятся авторизационной ключ 
        /// </summary>
        /// <param name="userAuthArgs">Модель параметров для входа</param>
        /// <returns>JWT Token</returns>
        public async Task<TokenDto> LoginAsync(UserAuthArgs userAuthArgs)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => false;
                    var resultTokenResponse = await GetTokenResponse(userAuthArgs, client);
                    TokenResponse tokenResponse = resultTokenResponse;
                    return new TokenDto {AccessToken = tokenResponse.AccessToken, RefreshToken = tokenResponse.RefreshToken, ExpiresIn = tokenResponse.ExpiresIn };
                }
            }
        }

        /// <summary>
        /// Обновить токен авторизации, когда он подойдет к концу действия
        /// </summary>
        /// <param name="refreshToken">Токен</param>
        /// <returns>Новый токен авторизации</returns>
        public async Task<TokenDto> RefreshTokenAsunc(string refreshToken)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => false;
                    var resultTokenResponse = await GetRefreshTokenResponse(refreshToken, client);
                    TokenResponse tokenResponse = resultTokenResponse;
                    return new TokenDto { AccessToken = tokenResponse.AccessToken, RefreshToken = tokenResponse.RefreshToken, ExpiresIn = tokenResponse.ExpiresIn};
                }
            }
        }

        private async Task<TokenResponse> GetRefreshTokenResponse(string token, HttpClient client)
        {
            var hostIdentityServer4 = _configuration["IdentityServer4:Options:Authority"];
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = hostIdentityServer4,
                Policy =
                {
                    RequireHttps = false
                }
            });
            if (disco.IsError)
            {
                throw new UserMessageException(disco.Error);
            }

            // request token
            TokenResponse tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "MonicaCrmClient",
                ClientSecret = "secret",
                RefreshToken = token,
                Scope = "api1",
            });


            if (tokenResponse.IsError)
            {
                throw new UserMessageException(tokenResponse.Error);
            }

            return tokenResponse;
        }


        /// <summary>
        /// Возвращает TokenResponse, или описание ошибки
        /// </summary>
        /// <param name="userAuth">Логин и пароль пользователя</param>
        /// <param name="client">HttpClient</param>
        /// <returns></returns>
        private async Task<TokenResponse> GetTokenResponse(UserAuthArgs userAuth, HttpClient client)
        {
            var hostIdentityServer4 = _configuration["IdentityServer4:Options:Authority"];
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = hostIdentityServer4,
                Policy =
                {
                    RequireHttps = false
                }
            });
            if (disco.IsError)
            {
                throw new UserMessageException(disco.Error);
            }

            // request token
            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "MonicaCrmClient",
                ClientSecret = "secret",

                UserName = userAuth.Login,
                Password = userAuth.Password,
                Scope = "api1 offline_access"
            });
            
            if (tokenResponse.IsError)
            {
                throw new UserMessageException(tokenResponse.Error);
            }

            return tokenResponse;
        }
    }
}
