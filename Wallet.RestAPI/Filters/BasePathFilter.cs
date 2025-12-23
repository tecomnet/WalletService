using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wallet.RestAPI.Filters
{
    /// <summary>
    /// BasePath Document Filter sets BasePath property of Swagger and removes it from the individual URL paths
    /// </summary>
    public class BasePathFilter : IDocumentFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath">BasePath to remove from Operations</param>
        public BasePathFilter(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Gets the BasePath of the Swagger Doc
        /// </summary>
        /// <returns>The BasePath of the Swagger Doc</returns>
        public string BasePath { get; }

        /// <summary>
        /// Apply the filter
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument</param>
        /// <param name="context">FilterContext</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers.Add(item: new OpenApiServer() { Url = this.BasePath });
            var pathsToModify = swaggerDoc.Paths.Where(predicate: p => p.Key.StartsWith(value: this.BasePath)).ToList();
            foreach (var path in pathsToModify)
            {
                if (!path.Key.StartsWith(value: this.BasePath))
                {
                    continue;
                }

                var newKey = Regex.Replace(input: path.Key, pattern: $"^{this.BasePath}", replacement: string.Empty);
                swaggerDoc.Paths.Remove(key: path.Key);
                swaggerDoc.Paths.Add(key: newKey, value: path.Value);
            }
        }
    }
}
