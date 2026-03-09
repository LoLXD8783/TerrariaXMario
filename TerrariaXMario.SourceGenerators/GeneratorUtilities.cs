using Microsoft.CodeAnalysis;

namespace TerrariaXMario.SourceGenerators;

enum ContainerType
{
    None,
    ModPlayer,
    JumpVariationPlayer,
}

internal record struct ClassInfo(string Namespace, string Name);
internal record struct FieldInfo(string Name, string Type, ClassInfo Container, ContainerType ContainerType, params object[] ExtraData);

internal static class GeneratorHelper
{
    internal static bool IsAssignableTo(ITypeSymbol type, string fullName, bool checkInterfaces)
    {
        ITypeSymbol tested = type;

        while (tested != null)
        {
            if (IsType(tested, fullName)) return true;
            tested = tested.BaseType;
        }

        if (checkInterfaces)
        {
            foreach (var interfaceType in type.Interfaces)
            {
                if (IsAssignableTo(interfaceType, fullName, true)) return true;
            }
        }

        return false;
    }

    private static bool IsType(ITypeSymbol symbol, string fullName)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == $"global::{fullName}";
    }
}