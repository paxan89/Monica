namespace Monica.Core.DbModel.ModelsAuth
{
    /// <summary>
    /// Модель для получения параметров от клиента с логином и пароля пользователя для авторизации
    /// </summary>
    public class UserAuthArgs
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
    }
}
