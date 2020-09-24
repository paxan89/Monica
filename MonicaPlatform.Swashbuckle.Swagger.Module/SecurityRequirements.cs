using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Platform.Swashbuckle.Swagger.Module
{
	public class SecurityRequirements : IOperationFilter
	{

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType != null)
            {
                var requiredScopes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .Union(context.MethodInfo.GetCustomAttributes(true))
                    .OfType<AuthorizeAttribute>()
                    .Select(a => a.Policy);

                if (!requiredScopes.Any())
                {
                    return;
                }
            }

            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized request. This may be because the request to the service has not been properly authenticated" });
            }
            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden request. This may be because the partner credentials are incorrect for the resource requested" });
            }
            if (!operation.Responses.ContainsKey("405"))
            {
                operation.Responses.Add("405", new OpenApiResponse { Description = "Not allowed request. This may be because the partner is not configured for the type of request" });
            }
            operation.Security = new List<OpenApiSecurityRequirement>();
		}
    }
}
