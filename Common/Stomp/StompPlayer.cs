using TerrariaXMario.Utilities.AssetData;

namespace TerrariaXMario.Common.Stomp;

[CanBeReadBySourceGenerators]
internal partial class StompPlayer : ModPlayer
{
    [NetSync] internal int stompCount;

    public override void PostUpdate()
    {
        if (Player.IsOnGroundPrecise || Player.mount.Active) stompCount = 0;

        if (!Player.CapPlayer.Enabled || Player.velocity.Y <= 0 || Player.mount.Active) return;

        // Adapted from Player.JumpMovement, specifically when using the Golf Cart mount

        Rectangle stompHitboxRect = Player.getRect();
        stompHitboxRect.Offset(0, Player.height - 1);
        stompHitboxRect.Height = 2;
        stompHitboxRect.Inflate(6, 6);
        bool groundPound = Player.GroundPoundPlayer.IsGroundPounding;

        for (int i = 0; i < 200; i++)
        {
            NPC npc = Main.npc[i];
            if (!npc.active || npc.dontTakeDamage || npc.friendly || npc.immune[Player.whoAmI] != 0 || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc)) continue;

            if (stompHitboxRect.Intersects(npc.getRect()) && (npc.noTileCollide || Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height)))
            {
                int velocityDirection = groundPound ? Player.direction : Math.Sign(Player.velocity.X);

                if (Player.whoAmI == Main.myPlayer) Player.ApplyDamageToNPC(npc, groundPound ? 2 : 1, groundPound ? 4 : 0, velocityDirection != 0 ? velocityDirection : Player.direction, true);

                npc.immune[Player.whoAmI] = 10;
                StompImpactDust.Spawn(Player);

                if (!groundPound)
                {
                    Player.velocity.Y = (Player.jumpSpeed + Player.jumpSpeedBoost) * -Player.gravDir * (Player.controlJump ? 2 : 1.5f);
                    Assets.Stomp.Play(Player.MountedCenter, pitch: Math.Clamp(stompCount, 0, 6) * 0.165f);

                    if (stompCount > 6)
                    {
                        Player.Heal(1);
                        Assets.Heal.Play(Player.MountedCenter);
                    }

                    stompCount++;

                    if (Player.LeanJumpPlayer.CanUpdate()) Player.LeanJumpPlayer.angle *= -1;
                    if (Player.FlipJumpPlayer.CanUpdate()) Player.FlipJumpPlayer.Init(240, Player.Size * 0.5f);
                }

                Player.GiveImmuneTimeForCollisionAttack(groundPound ? 24 : 6);
                break;
            }
        }
    }
}
