namespace Monica.Core.ModelParametrs.ModelsArgs
{
	/// <summary>
	/// Входная модель для смены пароля
	/// </summary>
	public class ChangePasswordArgs
	{
		/// <summary>
		/// Текущий пароль
		/// </summary>
		public string CurrentPassword { get; set; }

		/// <summary>
		/// Новый пароль
		/// </summary>
		public string NewPassword { get; set; }
	}
}
