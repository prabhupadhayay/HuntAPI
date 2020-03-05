using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiscoveryHuntApi.SwaggerConfiguration
{
    /// <summary>
    /// SwaggerParameterAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SwaggerParameterAttribute : Attribute
    {
        /// <summary>
        /// SwaggerParameterAttribute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public SwaggerParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; } = "text";
        /// <summary>
        /// Required
        /// </summary>
        public bool Required { get; set; } = false;
    }
}