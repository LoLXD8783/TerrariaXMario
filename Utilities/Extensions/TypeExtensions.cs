namespace TerrariaXMario.Utilities.Extensions;

internal static class TypeExtensions
{
    extension(Type type)
    {
        internal string NamespaceAsPath => type.Namespace!.Replace(".", "/");
    }
}
