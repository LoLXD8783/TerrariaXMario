using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.Emotes;

internal abstract class TownEmote : ModEmoteBubble
{
    public override string Texture => $"{GetType().Namespace!.Replace(".", "/")}/NPCEmotes";

    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.Town);
    }

    internal virtual int Row => 0;

    public override Rectangle? GetFrame()
    {
        return new Rectangle(EmoteBubble.frame * 34, 28 * Row, 34, 28);
    }

    public override Rectangle? GetFrameInEmoteMenu(int frame, int frameCounter)
    {
        return new Rectangle(frame * 34, 28 * Row, 34, 28);
    }
}

internal class ToadEmote : TownEmote
{
    public override void OnSpawn()
    {
        EmoteBubble.lifeTime *= 2;
        EmoteBubble.lifeTimeStart *= 2;
    }

    internal override int Row => 0;
}