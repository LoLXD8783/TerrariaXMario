namespace TerrariaXMario.Utilities.Extensions;

internal static partial class PlayerExtensions
{
    extension(Player player)
    {
        internal bool IsOnGroundPrecise => player.velocity.Y == 0 && (Collision.SolidCollision(player.BottomLeft, player.width, 1, true) || Collision.WaterCollision(player.BottomLeft, player.velocity, player.width, 1, lavaWalk: player.waterWalk2).Y == 0);
    }
}
