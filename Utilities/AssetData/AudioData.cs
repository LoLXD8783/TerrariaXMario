using ReLogic.Utilities;
using Terraria.Audio;

namespace TerrariaXMario.Utilities.AssetData;

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
    // Normal Gameplay
    internal AudioGroupData Equip { get; set; }
    internal AudioGroupData Unequip { get; set; }
    internal AudioGroupData DefeatBoss { get; set; }
    internal AudioGroupData Grab { get; set; }
    internal AudioGroupData Throw { get; set; }
    internal AudioGroupData Spin { get; set; }
    internal AudioGroupData WallKick { get; set; }
    // Death
    internal AudioGroupData Death { get; set; }
    internal AudioGroupData DeathElectric { get; set; }
    internal AudioGroupData DeathHot { get; set; }
    internal AudioGroupData DeathPoison { get; set; }
    internal AudioGroupData DeathBoss { get; set; }
    // Hurt
    internal AudioGroupData Hurt { get; set; }
    internal AudioGroupData HurtElectric { get; set; }
    // Stomp
    internal AudioGroupData StompGood { get; set; }
    internal AudioGroupData StompGreat { get; set; }
    // Jump
    internal AudioGroupData Jump { get; set; }
    internal AudioGroupData GroundPoundJump { get; set; }
    internal AudioGroupData DoubleJump { get; set; }
    internal AudioGroupData TripleJump { get; set; }
    internal AudioGroupData JumpPowerStar { get; set; }
    // Powerup
    internal AudioGroupData PowerUp { get; set; }
    internal AudioGroupData GetPowerStar { get; set; }
}