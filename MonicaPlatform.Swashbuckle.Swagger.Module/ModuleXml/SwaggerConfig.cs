namespace MonicaPlatform.Swashbuckle.Swagger.Module.ModuleXml
{
    public class SwaggerConfig
    {
        public string SwaggerBasePath { get; set; }
        public SwaggerEndpoint[] SwaggerEndpoints { get; set; }
    }

    public class SwaggerEndpoint
    {
        public string SwaggerEndpointUrl { get; set; }
        public string SwaggerEndpointName { get; set; }
    }
}
