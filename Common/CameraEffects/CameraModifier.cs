using Terraria.Graphics.CameraModifiers;

namespace TerrariaXMario.Common.CameraEffects
{
    internal class CameraModifier(Vector2 position, int frames, string uniqueIdentity = null!) : ICameraModifier
    {
        private readonly int framesToLast = frames;
        private int framesElapsed;
        internal Vector2 targetPosition = position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

        public string UniqueIdentity { get; private set; } = uniqueIdentity;
        public bool Finished { get; private set; }

        public void Update(ref CameraInfo cameraInfo)
        {
            float progress = Utils.GetLerpValue(0, framesToLast, framesElapsed);

            float lerpAmount = progress switch
            {
                < 0.5f => Utils.Remap(progress, 0, 0.5f, 0, 1),
                > 0.8f => Utils.Remap(progress, 0.8f, 1f, 1, 0),
                _ => 1,
            };

            cameraInfo.CameraPosition = Vector2.Lerp(cameraInfo.CameraPosition, targetPosition, lerpAmount);

            if (!Main.gameInactive && !Main.gamePaused) framesElapsed++;
            if (framesElapsed >= framesToLast) Finished = true;
        }

        internal static void Activate(Vector2 startPosition, Vector2 direction, float strength, float vibrationCyclesPerSecond, int frames, float distanceFalloff = -1f)
        {
            PunchCameraModifier modifier = new(startPosition, direction, strength, vibrationCyclesPerSecond, frames, distanceFalloff, typeof(CameraModifier).FullName);
            Main.instance.CameraModifiers.Add(modifier);
        }

        internal static void DoScreenShake(Player player, float strengthMultiplier = 1)
        {
            Activate(player.MountedCenter, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 2f * strengthMultiplier, 6f, 10, 1000f);
        }
    }
}