using ReLogic.Utilities;
using Terraria.Audio;

namespace TerrariaXMario.Utilities.Assets;

internal struct AudioData(string path)
{
    internal string Path = path;
    internal readonly SoundStyle Get => new(Path);

    internal readonly SlotId Play(Vector2? position = null, float volume = 1, float pitch = 0, SoundUpdateCallback? updateCallback = null)
    {
        SoundStyle style = Get;
        style.Volume = volume;
        style.Pitch = pitch;

        return SoundEngine.PlaySound(style, position, updateCallback);
    }
}

internal struct AudioGroupData(params string[] paths)
{
    internal string[] Paths = paths;
    internal readonly SoundStyle Get(int index) => new(Paths[index]);
    internal readonly AudioData GetData(int index) => new(Paths[index]);
    internal readonly AudioData GetRandomData => new(Main.rand.NextFromList(Paths));

    internal readonly SlotId Play(int index, Vector2? position, float volume = 1, float pitch = 0, SoundUpdateCallback? updateCallback = null) => GetData(index).Play(position, volume, pitch, updateCallback);
    internal readonly SlotId PlayRandom(Vector2? position, float volume = 1, float pitch = 0, SoundUpdateCallback? updateCallback = null) => GetRandomData.Play(position, volume, pitch, updateCallback);
}

internal struct CapAudioData
{
    internal AudioGroupData Equip { get; set; }
    internal AudioGroupData Unequip { get; set; }
    internal AudioGroupData Hurt { get; set; }
    internal AudioGroupData Die { get; set; }
    internal AudioGroupData Sick { get; set; }
    internal AudioGroupData DoubleJump { get; set; }
    internal AudioGroupData TripleJump { get; set; }
    internal AudioGroupData GroundPoundJump { get; set; }
}