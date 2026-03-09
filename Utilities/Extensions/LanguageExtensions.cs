using Terraria.Localization;

namespace TerrariaXMario.Utilities.Extensions;

internal static class LanguageExtensions
{
    extension(Language)
    {
        internal static string ModKey => $"Mods.{nameof(TerrariaXMario)}";
        internal static string FullKey(string key) => $"{Language.ModKey}.{key}";

        internal static string GetValue(string key) => Language.GetTextValue(FullKey(key));
        internal static string GetValue(string key, params object[] args) => Language.GetTextValue(FullKey(key), args);
        internal static LocalizedText Get(string key) => Language.GetText(FullKey(key));
        internal static LocalizedText Get(string key, params object[] args) => Language.GetText(FullKey(key)).WithFormatArgs(args);
    }
}
