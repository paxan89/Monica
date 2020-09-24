using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Monica.Core.Events;
using MonicaPlatform.Swashbuckle.Swagger.Module.ModuleXml;

namespace MonicaPlatform.Swashbuckle.Swagger.Module.OnStartupEvents
{
    public class OnStartupEvents : IOnStartupEvents
    {
        private readonly SwaggerConfig _configModel;

        public OnStartupEvents()
        {
            var builder = new ConfigurationBuilder().AddXmlFile(Path.Combine(
                Path.GetDirectoryName(typeof(OnStartupEvents).Assembly.Location),
                $"{typeof(OnStartupEvents).Assembly.GetName().Name}.dll.config"));
            var conf = builder.Build();
            _configModel = new SwaggerConfig();
            conf.GetSection("SwaggerConfig").Bind(_configModel);
        }

        /// <summary>
	    /// Startup(IConfiguration configuration)
	    /// </summary>
	    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
	    public void OnStartup(IConfiguration configuration)
        {
            //Console.WriteLine(nameof(OnStartup));
        }
        
        /// <summary>
        /// Перед вызовом services.AddMvc();
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services)
        {
            //Console.WriteLine(nameof(OnConfigureServicesBeforeAddMvc));
        }

        public void OnInitBackendService(IServiceCollection services)
        {
        }

        /// <summary>
        /// После вызова services.AddMvc();
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="mvcBuilder">An interface for configuring MVC services.</param>
        /// <param name="configuration"></param>
        public void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder,
            IConfiguration configuration = null)
        {
            //var filePath1 = Path.Combine((new AssemblyDirectory()).AssemblyDirectoryGet, "Portal.MainMenu.WebApi.xml");
            //var filePath2 = Path.Combine((new AssemblyDirectory()).AssemblyDirectoryGet, "Portal.MyMessages.WebApi.xml");


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo());

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });


                foreach (string filepath in Monica.Core.LoaderModules.LoaderExtensions.XmlDocs)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(filepath)) continue;
                        if (File.Exists(filepath))
                        {
                            if (!CheckXml(filepath)) continue;
                            Console.WriteLine(@"Файл прошел проверку XML. Добавляем файл комментария: " + filepath);
                            c.IncludeXmlComments(filepath);
                            c.CustomSchemaIds(x => x.FullName);
                            Console.WriteLine(@"Добавлен");
                        }
                        else
                            Console.WriteLine(@"Не найден файл: " + filepath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(@"Ошибка при добавлении файла XML комментария: " + filepath);
                        Console.WriteLine(e);
                    }

                }


            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        /// <summary>
        /// Перед if (env.IsDevelopment())
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Represents a type used to configure the logging system and create instances of ILogger from the registered ILoggerProviders.</param>
        /// <param name="iLoggerFactory">Represents a type used to perform logging.</param>
        public void OnConfigureBefore(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
            Console.WriteLine(nameof(OnConfigureBefore));
        }

        /// <summary>
        /// После if (env.IsDevelopment()) и перед app.UseMvc();
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Represents a type used to configure the logging system and create instances of ILogger from the registered ILoggerProviders.</param>
        /// <param name="iLoggerFactory">Represents a type used to perform logging.</param>
        public void OnConfigureAfter(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
            Console.WriteLine(nameof(OnConfigureAfter));
        }

        /// <summary>
        /// После app.UseMvc();
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Represents a type used to configure the logging system and create instances of ILogger from the registered ILoggerProviders.</param>
        /// <param name="iLoggerFactory">Represents a type used to perform logging.</param>
        public void OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add(Item);
            });


            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                var count = 0;
                try
                {
                    for (int i = 0; i < _configModel.SwaggerEndpoints?.Length; i++)
                    {
                        var cc = _configModel.SwaggerEndpoints[i];
                        c.SwaggerEndpoint(cc.SwaggerEndpointUrl, cc.SwaggerEndpointName);
                        count++;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    if (count != 0) return;
                    // Не задано конфигурации
                    Console.WriteLine(@"В файле конфигурации не задано ни одной SwaggerEndpoint!");
                    Console.WriteLine(@"Применяем стандартную: url = /swagger/v1/swagger.json, Name = My API V1");
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "localhost");
                    //c.SwaggerEndpoint("/portal/swagger/v1/swagger.json", "My API V1");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            });


        }

        private void Item(OpenApiDocument arg1, HttpRequest arg2)
        {

            arg1.Servers.Add(new OpenApiServer { Url = _configModel.SwaggerBasePath });
            arg2.PathBase = _configModel.SwaggerBasePath;
            //swagger.BasePath = _configModel.SwaggerBasePath;
        }

        public void OnConfigureAfterUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
        }


        /// <summary>
        /// Проверка XML
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool CheckXml(string filename)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                using (XmlReader reader = XmlReader.Create(streamReader))
                {
                    reader.MoveToContent();
                    while (reader.Read())
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Файл не прошел проверку XML:" + filename);
                Console.WriteLine(e);
                return false;
            }
            return true;
        }


    }
}
