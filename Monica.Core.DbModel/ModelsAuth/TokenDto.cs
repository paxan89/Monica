namespace Monica.Core.DbModel.ModelsAuth
{
    /// <summary>
    /// Класс токенов, который будет отправляться клиенту. Содержит пару Access и Refresh токен
    /// </summary>
    public class TokenDto
    {
        /// <summary>
        /// Токен авторизации. Основной
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// Токен обновления основного токена
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// Время жизни токена в секундах
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
