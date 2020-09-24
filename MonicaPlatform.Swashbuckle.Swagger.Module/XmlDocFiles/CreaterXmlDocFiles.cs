using System;
using System.Collections.Generic;
using System.Reflection;
using Platform.Swashbuckle.Swagger.Module.XmlDocFiles;

namespace MonicaPlatform.Swashbuckle.Swagger.Module.XmlDocFiles
{
	/// <summary>
	/// Создает объекты реализующие интефейс IXmlDocFiles
	/// </summary>
	public class CreaterXmlDocFiles
    {
		/// <summary>
		/// Тип с именем и реализацей наследованной от ILogModuleExternal
		/// </summary>
		private Type TypeIXmlDocFiles { get; set; } = null;

		/// <summary>
		/// Информация о свойстве ILogModuleExternal.LogComponents
		/// </summary>
		private PropertyInfo PropertyInfoListFilesXmlDoc { get; set; } = null;

		/// <summary>
		/// Список найденных интерфейсов
		/// </summary>
		private readonly List<Tuple<Type,PropertyInfo>> _listInterfaces = new List<Tuple<Type, PropertyInfo>>();

		/// <summary>
		/// Список коллекций.
		/// </summary>
		private readonly List<IReadOnlyCollection<string>> _listIReadOnlyCollection = new List<IReadOnlyCollection<string>>();


		/// <summary>
		/// Возвращаем список найденных коллекций файлов с XML документацией
		/// </summary>
		/// <returns></returns>
		public IReadOnlyCollection<IReadOnlyCollection<string>> Create()
	    {
		    try
		    {
			    if (!FindInterface()) return _listIReadOnlyCollection;
			    foreach (Tuple<Type, PropertyInfo> tuple in _listInterfaces)
			    {
				    // Создаем объект
				    Object objectIXmlDocFiles = CreateIXmlDocFiles(tuple.Item1);
				    PropertyInfo propertyInfoListFilesXmlDoc = tuple.Item2;
				    if (objectIXmlDocFiles is null) continue;
				    IReadOnlyCollection<string> iReadOnlyCollection = GetFieldValueListFilesXmlDoc(objectIXmlDocFiles, propertyInfoListFilesXmlDoc);
				    if (iReadOnlyCollection is null) continue;
					Console.WriteLine(@"Получена коллекция, тип данных: " + objectIXmlDocFiles.GetType().FullName);
				    Console.WriteLine(@"Файлов: " + iReadOnlyCollection.Count);
					_listIReadOnlyCollection.Add(iReadOnlyCollection);
			    }
			}
		    catch (Exception e)
		    {
			    Console.WriteLine(e);
		    }

		    return _listIReadOnlyCollection;
	    }

		#region Получение сведений об интерфейсе IXmlDocFiles

		/// <summary>
		/// Поиск интерфейса ILogModuleExternal
		/// </summary>
		private bool FindInterface()
		{
			Console.WriteLine(@"Поиск интерфейса IXmlDocFiles.");
			Assembly[] assembliesall = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assembliesall)
			{
				Type[] types = null;
				try
				{
					types = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException reflectionTypeLoadException)
				{
					// Сборка содержит один или несколько типов, которые не удается загрузить. 
					// Массив, возвращаемый свойством Types этого исключения, содержит объект 
					// Type для каждого типа, который был загружен, и объект null для каждого типа, 
					// который не удалось загрузить, тогда как свойство LoaderExceptions содержит 
					// исключение для каждого типа, который не удалось загрузить.
					// У меня возникало когда, не все зависимости (сборки, dll), были загружены, для данной сборки.
					Console.WriteLine(@"Сборка содержит один или несколько типов, которые не удается загрузить.");
					Console.WriteLine(@"Возникало когда, не все зависимости (сборки, dll), были загружены, для данной сборки.");
					Console.WriteLine(reflectionTypeLoadException);
				}
				catch (Exception e)
				{
					Console.WriteLine(@"Не удалось загрузить типы сборки.");
					Console.WriteLine(e);
				}
				// Не удалось получить типы из сборки.
				if (types is null)
				{
					Console.WriteLine(@" Не удалось получить типы из сборки.");
					continue;
				}

				//Type resultType = null;
				foreach (Type type in types)
				{
					Type[] interfaceTypes = null;
					// Устанавливаем все свойства интерфейса NULL
					SetInterfaceNull();
					try
					{
						interfaceTypes = type.GetInterfaces();
					}
					catch (TargetInvocationException targetInvocationException)
					{
						// Вызван статический инициализатор, выбрасывающий исключение.
						Console.WriteLine(@"Вызван статический инициализатор, выбрасывающий исключение.");
						Console.WriteLine(targetInvocationException);
						continue;
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						continue;
					}
					foreach (Type interfaceType in interfaceTypes)
					{
						//if (!interfaceType.IsClass) continue;
						if (!interfaceType.Name.Equals(nameof(IXmlDocFiles))) continue;
						try
						{
							Console.WriteLine(@"Найден интерфейс: " + nameof(IXmlDocFiles));

							TypeIXmlDocFiles = type;

							// Получаем свойства
							GetPropertesIXmlDocFiles(interfaceType);

							if (AnyInInterfaceIsNull())
							{
								_listInterfaces.Add(new Tuple<Type, PropertyInfo>(TypeIXmlDocFiles, PropertyInfoListFilesXmlDoc));
							}
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
						}
					}
					//if (AnyInInterfaceIsNull()) break;
				}
				//if (AnyInInterfaceIsNull()) break;
			}
			// Найдены интерфейсы
			return _listInterfaces.Count > 0;
		}

		/// <summary>
		/// проверяем все параметры интерфейса получили?
		/// т.е. есть хотябы один не полученный параметр интерфейса
		/// </summary>
		/// <returns></returns>
		private bool AnyInInterfaceIsNull()
		{
			// проверяем все параметры интерфейса получили?
			return !(TypeIXmlDocFiles is null) &&
				   !(PropertyInfoListFilesXmlDoc is null);
		}

		/// <summary>
		/// Устанавливаем все свойства интерфейса NULL
		/// </summary>
		private void SetInterfaceNull()
		{
			TypeIXmlDocFiles = null;
			PropertyInfoListFilesXmlDoc = null;
		}




		/// <summary>
		/// Получаем свойства интерфейса IXmlDocFiles.ListFilesXmlDoc
		/// </summary>
		/// <param name="interfaceType"></param>
		private void GetPropertesIXmlDocFiles(Type interfaceType)
		{
			try
			{
				// Ищем свойство IXmlDocFiles.ListFilesXmlDoc
				PropertyInfoListFilesXmlDoc = interfaceType.GetProperty(nameof(IXmlDocFiles.ListFilesXmlDoc));
				if (PropertyInfoListFilesXmlDoc is null)
				{
					Console.WriteLine("Не найдено свойство: " + nameof(IXmlDocFiles.ListFilesXmlDoc));
				}
				else
					Console.WriteLine("Найдено свойство: " + nameof(IXmlDocFiles.ListFilesXmlDoc));
			}
			catch (AmbiguousMatchException ambiguousMatchException)
			{
				// https://msdn.microsoft.com/ru-ru/library/kz0a8sxy(v=vs.110).aspx
				// Найдено несколько свойств с указанным именем. 
				Console.WriteLine(nameof(IXmlDocFiles.ListFilesXmlDoc));
				Console.WriteLine(@"Найдено несколько свойств с указанным именем.");
				Console.WriteLine(ambiguousMatchException);
			}
			catch (ArgumentNullException argumentNullException)
			{
				// Свойство name имеет значение null.
				Console.WriteLine(nameof(IXmlDocFiles.ListFilesXmlDoc));
				Console.WriteLine(@"Свойство name имеет значение null.");
				Console.WriteLine(argumentNullException);
			}
			catch (Exception e)
			{
				Console.WriteLine(nameof(IXmlDocFiles.ListFilesXmlDoc));
				Console.WriteLine(e);
			}
		}

		#endregion Получение сведений об интерфейсе IXmlDocFiles

		/// <summary>
		/// Создает объект с интерфейсом IXmlDocFiles
		/// </summary>
		private object CreateIXmlDocFiles(Type type)
		{
			if (type is null) return null;

			try
			{
				Console.WriteLine(@"Создаем объект реализующий интерфейс: " + nameof(IXmlDocFiles) + " Тип: " + type.FullName);
				Object objectIXmlDocFiles = Activator.CreateInstance(type);
				Console.WriteLine(@"Объект создан.");
				return objectIXmlDocFiles;
			}
			catch (ArgumentNullException argumentNullException)
			{
				// Свойство type имеет значение null.
				Console.WriteLine(@"Свойство type имеет значение null.");
				Console.WriteLine(argumentNullException);
			}
			catch (ArgumentException argumentException)
			{
				// type не является объектом RuntimeType.
				// Тип type является открытым универсальным типом (то есть свойство ContainsGenericParameters возвращает true).
				Console.WriteLine(@"type не является объектом RuntimeType.");
				Console.WriteLine(@"Тип type является открытым универсальным типом (то есть свойство ContainsGenericParameters возвращает true).");
				Console.WriteLine(argumentException);
			}
			catch (NotSupportedException notSupportedException)
			{
				// Тип type не может иметь значение TypeBuilder.
				// Создание типов TypedReference, ArgIterator, Void и RuntimeArgumentHandle или массивов этих типов не поддерживается.
				// Сборка, содержащая type, является динамической сборкой, которая была создана с использованием AssemblyBuilderAccess.Save.
				Console.WriteLine(@"Тип type не может иметь значение TypeBuilder.");
				Console.WriteLine(@"Создание типов TypedReference, ArgIterator, Void и RuntimeArgumentHandle или массивов этих типов не поддерживается.");
				Console.WriteLine(@"Сборка, содержащая type, является динамической сборкой, которая была создана с использованием AssemblyBuilderAccess.Save.");
				Console.WriteLine(notSupportedException);
			}
			catch (TargetInvocationException targetInvocationException)
			{
				// Вызываемый конструктор создает исключение.
				Console.WriteLine(@"Вызываемый конструктор создает исключение.");
				Console.WriteLine(targetInvocationException);
			}
			catch (MethodAccessException methodAccessException)
			{
				// В .NET for Windows Store apps или переносимой библиотеки классов, перехватывайте это исключение базовый класс MemberAccessException, вместо этого.
				// Вызывающий объект не имеет разрешения на вызов этого конструктора.
				Console.WriteLine(@"В .NET for Windows Store apps или переносимой библиотеки классов, перехватывайте это исключение базовый класс MemberAccessException, вместо этого.");
				Console.WriteLine(@"Вызывающий объект не имеет разрешения на вызов этого конструктора.");
				Console.WriteLine(methodAccessException);
			}
			catch (MemberAccessException memberAccessException)
			{
				// Невозможно создать экземпляр абстрактного класса, или этот элемент был вызван с помощь механизма позднего связывания.
				Console.WriteLine(@"Невозможно создать экземпляр абстрактного класса, или этот элемент был вызван с помощь механизма позднего связывания.");
				Console.WriteLine(memberAccessException);
			}
			catch (TypeLoadException typeLoadException)
			{
				// type не является допустимым типом.
				Console.WriteLine(@"type не является допустимым типом.");
				Console.WriteLine(typeLoadException);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
		}

		/// <summary>
		/// Возвращаем значение поля ListFilesXmlDoc
		/// </summary>
		private IReadOnlyCollection<string> GetFieldValueListFilesXmlDoc(Object objectIXmlDocFiles, PropertyInfo propertyInfoListFilesXmlDoc)
		{
			IReadOnlyCollection<string> iReadOnlyCollection = null;

			if (objectIXmlDocFiles is null) return null;
		    if (propertyInfoListFilesXmlDoc is null) return null;
		    try
		    {
			    Console.WriteLine(@"Получаем свойство " + nameof(IXmlDocFiles) + @"." + nameof(IXmlDocFiles.ListFilesXmlDoc));
			   iReadOnlyCollection = (IReadOnlyCollection<string>)propertyInfoListFilesXmlDoc.GetValue(objectIXmlDocFiles);
			    Console.WriteLine(@"Получено");
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine(e);
			    Console.WriteLine(@"Не удалось получить значение поля" + nameof(IXmlDocFiles) + @"." + nameof(IXmlDocFiles.ListFilesXmlDoc));
		    }

			return iReadOnlyCollection;

		}
	}
}
