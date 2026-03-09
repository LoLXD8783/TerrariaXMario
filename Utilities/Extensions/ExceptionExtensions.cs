using System.Runtime.CompilerServices;

namespace TerrariaXMario.Utilities.Extensions;

internal static class ExceptionExtensions
{
    extension(Exception)
    {
        internal static Exception QuickException => ExceptionWithCallerMemberName();
        private static Exception ExceptionWithCallerMemberName([CallerMemberName] string? caller = null) => new(caller);
    }
}
