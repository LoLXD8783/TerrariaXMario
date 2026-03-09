namespace TerrariaXMario.Utilities.Extensions;

internal static class StringExtensions
{
    extension(string String)
    {
        internal string JoinForPath(string string2) => $"{String}/{string2}";
        internal string JoinForPath(params string[] strings) => $"{String}/{string.Join("/", strings)}";
    }
}
