using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using TerrariaXMario.Utilities.Assets;

namespace TerrariaXMario.Common.PMeter;

[CanBeReadBySourceGenerators]
internal partial class PMeterPlayer : ModPlayer
{
    [NetSync] internal float chargeUpRate = 0.01f;
    [NetSync] internal float Charge { get; set { field = Math.Clamp(value, 0, 1); } }
    internal readonly int controlMoveReleaseTime = 15;
    [NetSync] internal int controlMoveReleaseTimer;
    [NetSync] internal bool fastRun;

    internal bool FullCharge => Charge == 1;

    internal SlotId PMeterFullSoundSlot;

    public override void Load()
    {
        // Removes all fast run particles, I will spawn them on my own whenever I want
        On_Player.SpawnFastRunParticles += DetourSpawnFastRunParticles;
    }

    private void DetourSpawnFastRunParticles(On_Player.orig_SpawnFastRunParticles orig, Player self)
    {
        if (!self.CapPlayer.Enabled) orig(self);
    }

    internal void DoFastRunEffects() // ripped from Player.SpawnFastRunParticles
    {
        if (!fastRun)
        {
            Assets.FastRunStart.Play(Player.MountedCenter);

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Player.MountedCenter, DustID.Cloud, Scale: 1.5f);
            }

            fastRun = true;
        }

        if (!Player.IsOnGroundPrecise) return;

        if (Player.runSoundDelay == 0)
        {
            SoundEngine.PlaySound(Player.hermesStepSound.Style, Player.position);
            Player.runSoundDelay = Player.hermesStepSound.IntendedCooldown;
        }

        int num = Player.gravDir == -1 ? -Player.height : 0;
        int num7 = Dust.NewDust(new Vector2(Player.position.X - 4f, Player.position.Y + Player.height + num), Player.width + 8, 4, DustID.Cloud, (0f - Player.velocity.X) * 0.5f, Player.velocity.Y * 0.5f, 50, Scale: 1.5f);
        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(Player.cShoe, Player);
    }

    public override void PostUpdateRunSpeeds()
    {
        if (!Player.CapPlayer.Enabled || Player.mount.Active)
        {
            Charge = 0;
            return;
        }

        // release timer for move inputs, used for maintaining full speed for some time when switching directions
        if (!Player.controlRight && !Player.controlLeft)
        {
            if (controlMoveReleaseTimer > 0) controlMoveReleaseTimer--;
        }
        else controlMoveReleaseTimer = controlMoveReleaseTime;

        if (Player.IsOnGroundPrecise && Math.Abs(Player.velocity.X) >= 2.5f && (Player.controlRight || Player.controlLeft)) Charge += chargeUpRate;

        if (controlMoveReleaseTimer == 0 || Player.GroundPoundPlayer.IsGroundPounding)
        {
            Charge = 0;
            fastRun = false;
        }

        if (Charge == 1)
        {
            if (Player.whoAmI == Main.myPlayer && !SoundEngine.TryGetActiveSound(PMeterFullSoundSlot, out _)) PMeterFullSoundSlot = Assets.PMeterFull.Play();

            DoFastRunEffects();
            Player.maxRunSpeed *= 1.5f;
        }
    }
}
