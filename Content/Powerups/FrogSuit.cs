using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class FrogSuit : Powerup
{
    internal override int? ProjectileType => ModContent.ProjectileType<FrogSuitProjectile>();
    internal override int? ItemType => ModContent.ItemType<FrogSuitItem>();
    internal override bool LookTowardRightClick => false;
    internal override Color Color => new(22, 176, 67);

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void UpdateConsumed(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        player.frogLegJumpBoost = true;
        player.autoJump = true;
        player.iceSkate = true;
        player.accDivingHelm = true;
        player.ignoreWater = true;
        player.waterWalk = modPlayer?.CurrentVariants.Contains("Running") ?? false;

        if (modPlayer == null) return;

        if (modPlayer.frogSwimming)
        {
            player.gravControl = player.gravControl2 = false;
            modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = null;
        }

        if (player.mount.Active) modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Running";
        else modPlayer.frogSwimming = player.wet;

        if (modPlayer.frogSwimming)
        {
            if (modPlayer.dashCooldown == 0)
            {
                Vector2 velocity = new Vector2((player.controlLeft ? -1 : 0) + (player.controlRight ? 1 : 0), ((player.controlUp || player.controlJump ? -1 : 0) + (player.controlDown ? 1 : 0)) * player.gravDir).SafeNormalize(Vector2.Zero) * 5f;

                player.velocity = velocity == Vector2.Zero ? Vector2.Lerp(player.velocity, velocity, 0.05f) : velocity;
            }

            player.gravity = 0;
        }

        if (!player.wet)
        {
            modPlayer.frogSwimFrame = 0;
            player.gravity = Player.defaultGravity;
        }
    }

    internal override void OnRightClick(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer != null && modPlayer.dashCooldown == 0)
        {
            modPlayer.PowerupCharge -= 50;
            modPlayer.dashCooldown = 30;
            Vector2 dashVelocity = player.MountedCenter.DirectionTo(Main.MouseWorld) * 12;
            dashVelocity.X = MathHelper.Clamp(dashVelocity.X, -10, 10);
            dashVelocity.Y = MathHelper.Clamp(dashVelocity.Y, -10, 10);
            player.velocity = dashVelocity;

            if (!modPlayer.frogSwimming) modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Running";
        }
    }
}

internal class FrogSuitProjectile : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<FrogSuit>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override string[] Variations => ["Running"];
    internal override bool CanSpawn(Player player) => player.ZoneBeach;
    internal override bool GroundPound => false;
}

internal class FrogSuitItem : PowerupItem
{
    internal override int? PowerupType => ModContent.GetInstance<FrogSuit>().Type;
}