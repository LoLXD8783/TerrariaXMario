using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;

internal static class PlayerExtensions
{
    public static T? GetModPlayerOrNull<T>(this Player player) where T : ModPlayer => player.TryGetModPlayer(out T result) ? result : null;

    public static bool IsOnGroundPrecise(this Player player)
    {
        return player.velocity.Y == 0 && (Collision.SolidCollision(player.BottomLeft, player.width, 1, true) || Collision.WaterCollision(player.BottomLeft, player.velocity, player.width, 1, lavaWalk: player.waterWalk2).Y == 0);
    }
}