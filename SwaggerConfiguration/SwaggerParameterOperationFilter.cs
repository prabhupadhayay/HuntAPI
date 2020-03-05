using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace DiscoveryHuntApi.SwaggerConfiguration
{
    /// <summary>
    /// SwaggerParameterOperationFilter
    /// </summary>
    public class SwaggerParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var requestAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerParameterAttribute>();
            if (requestAttributes.Any())
            {
                operation.parameters = operation.parameters ?? new List<Parameter>();

                foreach (var attr in requestAttributes)
                {
                    operation.parameters.Add(new Parameter
                    {
                        name = attr.Name,
                        description = attr.Description,
                        @in = attr.Type == "file" ? "formData" : "body",
                        required = attr.Required,
                        type = attr.Type,
                    });
                }

                if (requestAttributes.Any(x => x.Type == "file"))
                {
                    operation.consumes.Add("multipart/form-data");
                }
            }
        }
    }
}