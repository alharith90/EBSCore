using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
namespace EBSCore.Web.AppCode
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _resourcesPath;

        public JsonStringLocalizerFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _resourcesPath = _configuration["Localization:ResourcesPath"] ?? throw new ArgumentNullException("Resources path cannot be null.");
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_resourcesPath, resourceSource.FullName, CultureInfo.CurrentCulture);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_resourcesPath, baseName, CultureInfo.CurrentCulture);
        }

        public IStringLocalizer Create(string baseName)
        {
            return new JsonStringLocalizer(_resourcesPath, baseName, CultureInfo.CurrentCulture);
        }
    }
}