using Terraria.ModLoader;
using TerrariaXMario.Content.Projectiles;

namespace TerrariaXMario.Content.Powerups;

internal class IceFlower : FireFlower
{
    internal override int ProjectileType => ModContent.ProjectileType<IceFlowerIceball>();
    internal override float ProjectileGravity => 0.2f;
}