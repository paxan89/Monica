namespace Monica.Core.ModelParametrs.ModelsArgs
{
    /// <summary>
    /// Модель параметров для подтверждения адреса электронной почты пользователя
    /// </summary>
    public class ConfirmModelArgs
    {
        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Секретный ключ для подверждения адреса
        /// </summary>
        public string Code { get; set; }
    }
}
