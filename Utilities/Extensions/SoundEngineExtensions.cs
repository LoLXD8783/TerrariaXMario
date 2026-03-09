using ReLogic.Utilities;
using Terraria.Audio;

namespace TerrariaXMario.Utilities.Extensions;

internal static class SoundEngineExtensions
{
    extension(SoundEngine)
    {
        internal static SlotId PlaySound(string localPath, Vector2? position = null, float volume = 1, float pitch = 0) => SoundEngine.PlaySound(new($"{nameof(TerrariaXMario)}".JoinForPath(localPath))
        {
            Volume = volume,
            Pitch = pitch
        }, position);
    }
}