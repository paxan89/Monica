using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Logging;
using Monica.Core.Constants;
using Monica.Core.Events;

namespace Monica.PlatformMain.LoaderModules
{
    public static class TypeLoaderExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine(e);
                return e.Types.Where(t => t != null);
            }
        }
    }
    /// <summary>
    /// Описатели для работы с Assembly
    /// </summary>
    public static class LoaderExtensions
    {
        private static List<IOnProgramEvents> _onProgramEvents = new List<IOnProgramEvents>();
        private static List<IOnStartupEvents> _onStartupEvents = new List<IOnStartupEvents>();

        /// <summary>
        /// Загрузка всех модулей в Assembly, которые будут считаны из конфигурационного файла настроек
        /// </summary>
        public static void Load(string path = null)
        {
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            foreach (var file in files)
            {
                try
                {
                    if (file.IndexOf("testhost.dll", StringComparison.Ordinal) > -1)
                    {
                        continue;
                    }

                    System.Runtime.Loader.AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(file);
                }
                catch (Exception e)
                {
                    
                }
                
            }


            path = path ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GlobalSettingsApp.CurrentAppDirectory = path;
            var pathConfigFile = Path.Combine(path,
                "Monica.Core.dll.config");
            var config = XElement.Load(pathConfigFile);
            var mosules = config.Descendants("ModuleConfigLoader");
            foreach (var child in mosules)
            {
                try
                {
                    var moduleName = child.Attribute("name")?.Value;
                    var modulePath = child.Attribute("path")?.Value;
                    var isMvc = Convert.ToBoolean(child.Attribute("isMvc")?.Value ?? "false");
                    if (isMvc)
                        continue;
                    if (string.IsNullOrEmpty(modulePath))
                    {
                        Console.WriteLine($"Пустой путь при чтении модуля конфигурации {moduleName}");
                        continue;
                    }

                    var directories = Directory.GetDirectories(Path.Combine(path, modulePath));
                    foreach (var directory in directories)
                    {
                        try
                        {
                            var modules = GetFileModule(directory, out var fileName);
                            FileInfo file = new FileInfo(fileName);
                            LoaderAssembly(file, modules);
                            Console.WriteLine(file.FullName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Не удалось прочитать модуль для конфигурации {moduleName}. Текст ошибки {e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            OnProgramRun();
            OnStatupRun();
        }

        /// <summary>
        /// Загрузка всех MVC модулей в Assembly, которые будут считаны из конфигурационного файла настроек
        /// </summary>
        public static void LoadMvc(this IMvcBuilder mvcBuilder, string path = null)
        {
            path = path ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GlobalSettingsApp.CurrentAppDirectory = path;
            var pathConfigFile = Path.Combine(path,
                "Monica.Core.dll.config");
            var config = XElement.Load(pathConfigFile);
            var mosules = config.Descendants("ModuleConfigLoader");
            foreach (var child in mosules)
            {
                var moduleName = child.Attribute("name")?.Value;
                var modulePath = child.Attribute("path")?.Value;

                try
                {
                    if (string.IsNullOrEmpty(modulePath))
                    {
                        Console.WriteLine($"Пустой путь при чтении модуля конфигурации {moduleName}");
                        continue;
                    }
                    var directories = Directory.GetDirectories(Path.Combine(path, modulePath));
                    foreach (var directory in directories)
                    {
                        try
                        {
                            var modules = GetFileModule(directory, out var fileName);
                            FileInfo file = new FileInfo(fileName);
                            LoaderAssembly(file, modules, mvcBuilder);
                            Console.WriteLine(file.FullName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Не удалось прочитать модуль для конфигурации {moduleName}. Текст ошибки {e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


            }
        }

        private static IEnumerable<XElement> GetFileModule(string directory, out string fileName)
        {
            fileName = string.Empty;
            var files = Directory.GetFiles(directory, "*.xml");
            foreach (var file in files)
            {
                var elements = XElement.Load(file).Descendants("ModuleDll");
                var fileModule = elements as XElement[] ?? elements.ToArray();
                if (fileModule.Any())
                {
                    fileName = file;
                    return fileModule;
                }
            }

            return null;
        }

        #region Инициализация компонентов платформы

        /// <summary>
        /// Инициализация всех подгруженных классов, которые имеюют интерфейс IOnProgramEvents
        /// </summary>
        public static void OnProgramRun()
        {
            _onProgramEvents = GetInstanceObject<IOnProgramEvents>(GetAllTypes<IOnProgramEvents>()).ToList();
        }

        /// <summary>
        /// Инициализация всех подгруженных классов, которые имеюют интерфейс IOnStartupEvents
        /// </summary>
        public static void OnStatupRun()
        {
            _onStartupEvents = GetInstanceObject<IOnStartupEvents>(GetAllTypes<IOnStartupEvents>()).ToList();
        }
        /// <summary>
        /// Получить инициализированные объекты из типов
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private static IEnumerable<TModel> GetInstanceObject<TModel>(IEnumerable<Type> types) where TModel : class
        {
            return types.Where(type => type.IsClass).Select(type => Activator.CreateInstance(type) as TModel);
        }

        private static IEnumerable<Type> GetAllTypes<TType>()
        {
            IEnumerable<Type> types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      let type = assembly.GetLoadableTypes().FirstOrDefault(ass => typeof(TType).IsAssignableFrom(ass))
                                      where type != null
                                      select type;
            return types;
        }


        private static void LoaderAssembly(FileInfo pathConfig, IEnumerable<XElement> moduleLoaders, IMvcBuilder mvcBuilder = null)
        {
            foreach (var module in moduleLoaders)
            {
                var moduleName = module.Attribute("name")?.Value;
                var modulePath = module.Attribute("path")?.Value;
                try
                {
                    Console.WriteLine(pathConfig.FullName);
                    var isMvcLoad = Convert.ToBoolean(module.Attribute("isMvc")?.Value ?? "false");
                    //if (isMvcLoad && mvcBuilder == null)
                    //    continue;
                    //if (!isMvcLoad && mvcBuilder != null)
                    //    continue;
                    if (string.IsNullOrEmpty(modulePath))
                    {
                        Console.WriteLine($"Пустой путь при загрузке библиотеки модуля {moduleName}");
                        continue;
                    }

                    FileInfo file = new FileInfo(Path.Combine(pathConfig.DirectoryName, modulePath));
                    var ass = AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(file.FullName);

                    if (isMvcLoad)
                        mvcBuilder?.AddApplicationPart(ass);
                    Core.LoaderModules.LoaderExtensions.XmlDocs =
                        Core.LoaderModules.LoaderExtensions.XmlDocs ?? new List<string>();
                    if (file.Exists)
                    {
                        var fileXml = new FileInfo(Path.Combine(pathConfig.DirectoryName, modulePath.Replace(".dll", ".xml").Replace(".DLL", ".xml")));
                        if (fileXml.Exists)
                            Core.LoaderModules.LoaderExtensions.XmlDocs.Add(fileXml.FullName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Не удалось загрузить библиотеку {moduleName}");
                    Console.WriteLine(e);
                }
            }
        }

        #endregion

        #region IOnProgramEvents

        /// <summary>
        /// Событие которое будет вызвано в Main класса програм до старта сервиса
        /// </summary>
        /// <param name="args"></param>
        public static void OnProgramMainBeforeRun(string[] args)
        {
            foreach (var eventse in _onProgramEvents)
            {
                eventse.OnProgramMainBeforeRun(args);
            }
        }
        /// <summary>
        /// Событие которое будет вызвано в Main класса програм после старта сервиса
        /// </summary>
        public static void OnProgramMainAfterRun(string[] args)
        {
            foreach (var eventse in _onProgramEvents)
            {
                eventse.OnProgramMainAfterRun(args);
            }
        }

        /// <summary>
        /// Событие которое будет вызвано в методе Main класса программ после создания builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="iWebHostBuilder"></param>
        public static void OnProgramBuildWebHostAfterUseStartup(string[] args, IWebHostBuilder iWebHostBuilder)
        {
            foreach (var eventse in _onProgramEvents)
            {
                eventse.OnProgramBuildWebHostAfterUseStartup(args, iWebHostBuilder);
            }
        }


        /// <summary>
        /// Событие которое будет вызвано в методе Main класса программ после создания builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="webHost"></param>
        public static void OnProgramBuildWebHostAfterBuild(string[] args, IWebHost webHost)
        {
            foreach (var eventse in _onProgramEvents)
            {
                eventse.OnProgramBuildWebHostAfterBuild(args, webHost);
            }
        }



        /// <summary>
        /// Событие которое будет вызвано в методе Main класса программ после создания builder
        /// </summary>
        /// <param name="args"></param>
        public static void OnProgramBuildWebHostBeforeCreateDefaultBuilder(string[] args)
        {
            foreach (var eventse in _onProgramEvents)
            {
                eventse.OnProgramBuildWebHostBeforeCreateDefaultBuilder(args);
            }
        }
        /// <summary>
        /// Событие которое будет вызвано в методе Main класса программ lj создания builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="iWebHostBuilder"></param>
        public static void OnProgramBuildWebHostBeforeUseStartup(string[] args, IWebHostBuilder iWebHostBuilder)
        {
            foreach (var eventse in _onProgramEvents)
            {
                eventse.OnProgramBuildWebHostBeforeUseStartup(args, iWebHostBuilder);
            }
        }

        #endregion

        #region IOnStartupEvents

        /// <summary>
        /// Происходит в методе конфигурировании сервиса до вызова AddMVC
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="loggerFactory"></param>
        public static void OnConfigureBefore(IApplicationBuilder applicationBuilder,
            IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnConfigureBefore(applicationBuilder, hostingEnvironment, loggerFactory);
            }
        }
        /// <summary>
        /// происходит в методе конфигурировании сервиса после срабатывания AddMvc
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="loggerFactory"></param>
        public static void OnConfigureAfter(IApplicationBuilder applicationBuilder,
            IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnConfigureAfter(applicationBuilder, hostingEnvironment, loggerFactory);
            }
        }


        /// <summary>
        /// происходит в методе конфигурировании сервиса после срабатывания AddMvc
        /// </summary>
        public static void OnConfigureServicesBeforeAddMvc(IServiceCollection services)
        {
            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnConfigureServicesBeforeAddMvc(services);
            }
        }
        /// <summary>
        /// происходит в методе конфигурировании сервиса после срабатывания AddMvc
        /// </summary>
        public static void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder, IConfiguration configuration)
        {
            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnConfigureServicesAfterAddMvc(services, mvcBuilder, configuration);
            }
        }
        /// <summary>
        /// происходит в методе конфигурировании сервиса до срабатывания AddMvc
        /// </summary>
        public static void OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnConfigureBeforeUseMvc(app, env, iLoggerFactory);
            }
        }


        /// <summary>
        /// происходит в методе конфигурировании сервиса до срабатывания AddMvc
        /// </summary>
        public static void OnConfigureAfterUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnConfigureAfterUseMvc(app, env, iLoggerFactory);
            }
        }

        public static void OnInitBackendService(IServiceCollection services)
        {

            foreach (var eventse in _onStartupEvents)
            {
                eventse.OnInitBackendService(services);
            }
        }

        #endregion

    }

    class TestAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
