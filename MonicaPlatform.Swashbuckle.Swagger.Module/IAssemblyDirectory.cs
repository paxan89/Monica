namespace Platform.Swashbuckle.Swagger.Module
{
	/// <summary>
	/// Доступ к папке со сборкой
	/// </summary>
	public interface IAssemblyDirectory
	{
		/// <summary>
		/// Папка с текущей сборкой
		/// </summary>
		string AssemblyDirectoryGet { get; }
	}
}