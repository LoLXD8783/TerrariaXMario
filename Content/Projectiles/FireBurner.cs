using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.MetaballContent;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Core.Effects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Projectiles;

internal class FireBurner : MetaballProjectile, IDrawToDustMetaballsTarget
{
    internal override Color OutlineColor => new(249, 28, 26);
    internal override Color FillColor => new(246, 225, 21);
    internal override float Radius => 16;
    internal override int? PairedMetaballDust => ModContent.DustType<FireFlowerFireballDust>();

    internal int dustType = DustID.Torch;
    internal int buffType = BuffID.OnFire;
    internal int dustChance = 1;

    public void DrawToMetaballs(MetaballDust dustsThatWillBeDrawn, SpriteBatch sb, Texture2D metaballCircleTexture)
    {
        if (dustsThatWillBeDrawn.Type != PairedMetaballDust)
            return;
        Vector2 scale = new Vector2(20) / metaballCircleTexture.Size() * Projectile.scale * new Vector2(2, 1);
        sb.Draw(metaballCircleTexture, Projectile.Center - Main.screenPosition, null, Color.White * ((255 - Projectile.alpha) / 255), Projectile.rotation + MathHelper.PiOver2, metaballCircleTexture.Size() / 2, scale, SpriteEffects.None, 0);
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 2;
        Projectile.alpha = 255;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 directionTo = player.MountedCenter.DirectionTo(Main.MouseWorld);

        if (player.channel)
        {
            player.GetModPlayerOrNull<CapEffectsPlayer>()?.SetForceDirection(5, Math.Sign(Main.MouseWorld.X - player.MountedCenter.X));
            Projectile.timeLeft++;
            Projectile.Center = player.MountedCenter + directionTo * 32;
            Projectile.rotation = directionTo.ToRotation();
        }

        if (PairedMetaballDust != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.TopLeft, Projectile.width, Projectile.height, (int)PairedMetaballDust, Scale: 1.5f);
                dust2.velocity = directionTo * 8 + player.velocity;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(3)) target.AddBuff(buffType, 300);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        if (Main.rand.NextBool(3)) target.AddBuff(buffType, 300);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, dustType, Scale: 3f);
            dust.noGravity = true;
            dust.velocity *= 4f;
            Dust dust2 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Smoke, 0f, 0f);
            dust2.noGravity = true;
            dust2.velocity *= 4f;

            if (PairedMetaballDust == null) continue;

            Dust dust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)PairedMetaballDust);
            dust3.scale = Main.rand.NextFloat(0.7f, 0.8f) * Projectile.scale;
        }
    }
}
