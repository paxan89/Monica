namespace Monica.Core.ModelParametrs.ModelsArgs
{
	/// <summary>
	/// Входная модель для сброса пароля, по коду восстановления (выдается по e-mail)
	/// </summary>
	public class ResetPasswordArgs
	{
		/// <summary>
		/// e-mail
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Новый пароль
		/// </summary>
		public string NewPassword { get; set; }

		/// <summary>
		/// Подтверждение нового пароля
		/// </summary>
		public string ConfirmNewPassword { get; set; }
		/// <summary>
		/// Проверочный токен/код
		/// </summary>
		public string Code { get; set; }
	}
}
