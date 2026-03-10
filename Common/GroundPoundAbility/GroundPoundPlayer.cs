using Terraria.DataStructures;
using Terraria.GameInput;
using TerrariaXMario.Common.CameraEffects;
using TerrariaXMario.Common.StompAbility;
using TerrariaXMario.Content.Cap;
using TerrariaXMario.Utilities.Assets;

namespace TerrariaXMario.Common.GroundPoundAbility;

[CanBeReadBySourceGenerators]
internal partial class GroundPoundPlayer : ModPlayer
{
    internal bool IsGroundPounding => Player.CapPlayer.currentVariation == EquipSet.GroundPound.Name;

    [NetSync] private bool cancelledGroundPound;

    // Allows players to ground pound by holding Down while airborne
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (PlayerInput.Triggers.JustPressed.Down && !Player.IsOnGroundPrecise && Player.velocity.Y != 0 && !Player.mount.Active && !IsGroundPounding)
        {
            Player.CapPlayer.currentVariation = EquipSet.GroundPound.Name;
            //Player.JumpVariationPlayer.flipDuration = 12;
            Assets.GroundPoundStart.Play(Player.MountedCenter);
        }
    }

    // Player velocity modifications
    public override void PreUpdateMovement()
    {
        if (cancelledGroundPound) Player.velocity.Y = 0.1f * Player.gravDir;

        if (!IsGroundPounding) return;

        //Player.velocity = new Vector2(0, (Player.JumpVariationPlayer.flipTimer > 0 ? -0.1f : Player.maxFallSpeed) * Player.gravDir);
    }

    // Visual changes (flip and animations)
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (cancelledGroundPound)
        {
            Player.fullRotation = 0;
            cancelledGroundPound = false;
        }

        if (!IsGroundPounding) return;

        player.bodyFrame.Y = 0;
        //player.legFrame.Y = 56 * Math.Clamp((modPlayer.flipDuration - modPlayer.flipTimer) / (modPlayer.flipDuration / 3), 0, 3);
    }

    public override void PreUpdate()
    {
        if (IsGroundPounding && (!Player.CapPlayer.Enabled || Player.mount.Active || Player.IsOnGroundPrecise /*|| (Player.JumpVariationPlayer.flipTimer == 0 && !Player.controlDown)*/))
        {
            if (Player.IsOnGroundPrecise) // Player hits the ground during ground pound
            {
                Assets.GroundPound.Play(Player.MountedCenter);
                CameraModifier.DoScreenShake(Player);
                StompImpactDust.Spawn(Player, 4, 1.5f, -1, -0.5f);
            }
            else if (!Player.controlDown) cancelledGroundPound = true;

            CapPlayer.ResetVariation(Player);
        }
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        if (IsGroundPounding) return false;
        return base.CanBeHitByNPC(npc, ref cooldownSlot);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (IsGroundPounding) return false;
        return base.CanBeHitByProjectile(proj);
    }
}
