namespace Monica.Core.ModelParametrs.ModelsArgs
{
    /// <summary>
    /// Модель для регистрации пользователя в системе
    /// </summary>
    public class RegistrationUserArgs
    {
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string Middlename { get; set; }
        /// <summary>
        /// Телефон пользователя
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Подтвержденрие пароля
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}
