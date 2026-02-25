//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.ModLoader;
//using TerrariaXMario.Content.Caps;

//namespace TerrariaXMario.Content.Powerups;

//internal class BlueShell : Powerup
//{
//    internal override int? ProjectileType => ModContent.ProjectileType<BlueShellProjectile>();
//    internal override int? ItemType => ModContent.ItemType<BlueShellItem>();

//    internal override bool LookTowardRightClick => false;
//    internal override Color Color => new(30, 84, 195);
//    internal override void UpdateWorld(Projectile projectile, int updateCount)
//    {
//        projectile.velocity.Y += 0.4f;
//    }
//}

//internal class BlueShellProjectile : PowerupProjectile
//{
//    internal override int? PowerupType => ModContent.GetInstance<BlueShell>().Type;
//    internal override string[] Caps => [nameof(Luigi)];
//    internal override PowerupVisualVariation[] Variations => [new("", EquipType.Body, EquipType.Back)];
//}

//internal class BlueShellItem : PowerupItem
//{
//    internal override int? PowerupType => ModContent.GetInstance<BlueShell>().Type;
//}