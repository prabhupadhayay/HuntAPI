using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Description;

namespace DiscoveryHuntApi.SwaggerConfiguration
{
    /// <summary>
    /// ParameterFilter
    /// </summary>
    public class ParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.parameters.Where(x => x.@in == "query" && x.name.Contains(".")))
            {
                parameter.name = Regex.Replace(parameter.name,
                    @"^ # Match start of string
                .*? # Lazily match any character, trying to stop when the next condition becomes true
                \.  # Match the dot", "", RegexOptions.IgnorePatternWhitespace);
            }
        }
    }
}