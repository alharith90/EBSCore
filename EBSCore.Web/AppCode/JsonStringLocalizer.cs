using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace EBSCore.Web.AppCode
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly string _resourcesPath;
        private readonly string _baseName;
        private readonly CultureInfo _culture;

        private readonly Lazy<Dictionary<string, string>> _localizedStrings;

        public JsonStringLocalizer(string resourcesPath, string baseName, CultureInfo culture)
        {
            _resourcesPath = resourcesPath;
            _baseName = baseName;
            _culture = culture;

            _localizedStrings = new Lazy<Dictionary<string, string>>(LoadLocalizedStrings);
        }

        public LocalizedString this[string name] => GetString(name);

        public LocalizedString this[string name, params object[] args]
        {
            get
            {
                try
                {
                    var localizedString = GetString(name);

                    if (localizedString.ResourceNotFound)
                    {
                        return localizedString;
                    }

                    var formattedValue = string.Format(_culture ?? CultureInfo.CurrentCulture, localizedString.Value, args);
                    return new LocalizedString(name, formattedValue);
                }
                catch (Exception ex)
                {
                    try
                    {
                        new Common().LogError(ex, $"Localization formatting failed for key '{name}'");
                    }
                    catch
                    {
                    }

                    return new LocalizedString(name, name, true);
                }
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizedStrings.Value.Select(s => new LocalizedString(s.Key, s.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_resourcesPath, _baseName, culture);
        }

        private Dictionary<string, string> LoadLocalizedStrings()
        {
            var CultureString = _culture.Name.Split('-')[0];
            var filePath = Path.Combine($"{_resourcesPath}/{_baseName}.{CultureString}.json");
            var localizedStrings = new Dictionary<string, string>();

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }

            return localizedStrings;
        }

        private LocalizedString GetString(string name)
        {
            if (_localizedStrings.Value.TryGetValue(name, out var value))
                return new LocalizedString(name, value);

            return new LocalizedString(name, name, true);
        }
    }

}