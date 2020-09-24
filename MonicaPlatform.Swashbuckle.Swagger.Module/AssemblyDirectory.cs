using System;
using System.IO;
using System.Reflection;

namespace Platform.Swashbuckle.Swagger.Module
{
	/// <summary>
	/// Доступ к папке со сборкой
	/// </summary>
	public class AssemblyDirectory : IAssemblyDirectory
	{
		/// <summary>
		/// Папка с текущей сборкой
		/// </summary>
		public string AssemblyDirectoryGet
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);

				//return @"C:\Platform\Platform.Main\Dev\Platform.Main\Platform.Main\bin\x64\Debug\netcoreapp2.0\";
			}
		}
	}
}
