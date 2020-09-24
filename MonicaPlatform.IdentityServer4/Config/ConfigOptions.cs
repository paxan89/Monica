using System;
using System.Text;
using IdentityModel;
using MonicaPlatform.IdentityServer4.Models;

namespace MonicaPlatform.IdentityServer4.Config
{
	/// <summary>
	/// Класс опций
	/// </summary>
	internal class ConfigOptions
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="identityServer4Config">Загруженные данные из XML</param>
		public ConfigOptions(Configuration identityServer4Config)
		{
			Init(identityServer4Config);
		}

		/// <summary>
		/// Инициализация
		/// </summary>
		/// <param name="identityServer4Config"></param>
		private void Init(Configuration identityServer4Config)
		{
			if (identityServer4Config.Options is null)
			{
				Console.WriteLine(@"identityServer4Config.Options is null. Применяем параметры по умолчанию.");
				return;
			}

			// ApiName
			if (string.IsNullOrWhiteSpace(identityServer4Config.Options.ApiName))
				Console.WriteLine(@"ApiName - пуст!");
			else
				ApiName = identityServer4Config.Options.ApiName.Trim();

			// RequireHttpsMetadata
			if (!identityServer4Config.Options.RequireHttpsMetadata)
				Console.WriteLine(@"RequireHttpsMetadata - не задан!");
			else
				RequireHttpsMetadata = identityServer4Config.Options.RequireHttpsMetadata;

			// Authority
			if (string.IsNullOrWhiteSpace(identityServer4Config.Options.Authority))
				Console.WriteLine(@"Authority - пуст!");
			else if (Uri.TryCreate(identityServer4Config.Options.Authority.Trim(), UriKind.Absolute, out var uri))
				Authority = identityServer4Config.Options.Authority.Trim();
			else
				Console.WriteLine(@"Authority - ошибка при записи URL, принимаем значение по умолчанию!");

			#region Claims

			// NameClaimType
			if (string.IsNullOrWhiteSpace(identityServer4Config.Options.Claims.NameClaimType))
				Console.WriteLine(@"NameClaimType - пуст!");
			else
				NameClaimType = identityServer4Config.Options.Claims.NameClaimType.Trim();

			// RoleClaimType
			if (string.IsNullOrWhiteSpace(identityServer4Config.Options.Claims.RoleClaimType))
				Console.WriteLine(@"RoleClaimType - пуст!");
			else
				RoleClaimType = identityServer4Config.Options.Claims.RoleClaimType.Trim();

			#endregion Claims
		}

		/// <summary>
		/// Строковое описание конфигурации
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(nameof(Authority) + @" = " + Authority);
			stringBuilder.AppendLine(nameof(RequireHttpsMetadata) + @" = " + RequireHttpsMetadata);
			stringBuilder.AppendLine(nameof(ApiName) + @" = " + ApiName);
			// Claims
			stringBuilder.AppendLine(nameof(NameClaimType) + @" = " + NameClaimType);
			stringBuilder.AppendLine(nameof(RoleClaimType) + @" = " + RoleClaimType);

			return stringBuilder.ToString();
		}

		/// <summary>
		/// base-address of your identityserver
		/// </summary>
		public string Authority { get; private set; } = @"http://localhost:5000/";

		/// <summary>
		/// RequireHttpsMetadata
		/// </summary>
		public bool RequireHttpsMetadata { get; private set; } = false;

		/// <summary>
		/// Name of the API resource used for authentication against introspection endpoint
		/// </summary>
		public string ApiName { get; private set; } = @"api1";

		#region Claims

		/// <summary>
		/// Claim type for name
		/// </summary>
		public string NameClaimType { get; private set; } = JwtClaimTypes.GivenName;

		/// <summary>
		/// Claim type for role
		/// </summary>
		public string RoleClaimType { get; private set; } = JwtClaimTypes.Role;

		
		#endregion Claims
	}
}
