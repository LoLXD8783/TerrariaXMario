using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace TerrariaXMario.Utilities.Extensions;

internal static class ILCursorExtensions
{
    extension(ILCursor cursor)
    {
        internal void Next(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
        {
            if (!cursor.TryGotoNext(moveType, predicates)) throw Exception.QuickException;
        }

        internal void Next(params Func<Instruction, bool>[] predicates)
        {
            cursor.Next(MoveType.Before, predicates);
        }

        internal void Previous(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
        {
            if (!cursor.TryGotoPrev(moveType, predicates)) throw Exception.QuickException;
        }

        internal void Previous(params Func<Instruction, bool>[] predicates)
        {
            cursor.Previous(MoveType.Before, predicates);
        }
    }
}
