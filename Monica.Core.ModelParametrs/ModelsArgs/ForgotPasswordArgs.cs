namespace Monica.Core.ModelParametrs.ModelsArgs
{
	/// <summary>
	/// Входная модель для восстановления забытого пароля по e-mail
	/// </summary>
	public class ForgotPasswordArgs
	{
		/// <summary>
		/// e-mail
		/// </summary>
		public string Email { get; set; }
        /// <summary>
        /// Аккаунт пользователя
        /// </summary>
        public string UserName { get; set; }
	}
}
