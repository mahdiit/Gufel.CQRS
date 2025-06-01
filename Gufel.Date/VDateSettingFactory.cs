using System.Collections.Concurrent;
using Gufel.Date.Base;
using Gufel.Date.Setting;

namespace Gufel.Date
{
    public static class VDateSettingFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<VDateSetting>> Settings =
            new(StringComparer.OrdinalIgnoreCase);

        static VDateSettingFactory()
        {
            Register("ar", () => new VDateArabicSetting());
            Register("en", () => new VDateEnglishSetting());
            Register("fa", () => new VDatePersianSetting());
        }

        public static void Register(string langCode, Func<VDateSetting> creator)
        {
            if (string.IsNullOrWhiteSpace(langCode))
                throw new ArgumentException("Language code must not be null or whitespace.");

            Settings[langCode] = new Lazy<VDateSetting>(creator, isThreadSafe: true);
        }

        public static VDateSetting GetSetting(string langCode)
        {
            if (Settings.TryGetValue(langCode, out var setting))
            {
                return setting.Value;
            }

            throw new NotSupportedException($"Language setting for '{langCode}' not found.");
        }
    }
}