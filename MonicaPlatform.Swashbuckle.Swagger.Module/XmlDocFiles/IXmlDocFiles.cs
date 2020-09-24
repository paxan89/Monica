using System.Collections.Generic;

namespace Platform.Swashbuckle.Swagger.Module.XmlDocFiles
{
	/// <summary>
	/// Доступ к xml файлам документации Visual Studio 
	/// </summary>
    interface IXmlDocFiles
    {
		/// <summary>
		/// Возвращает файлы (абсолютный путь) XML документации сборок.
		/// </summary>
		IReadOnlyCollection<string> ListFilesXmlDoc { get; }
    }
}
