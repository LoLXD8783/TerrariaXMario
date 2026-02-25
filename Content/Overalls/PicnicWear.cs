using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class PicnicWear : OverallsItem
{
    internal override int Defense => 5;
    internal override int PowAdditive => 4;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(silver: 75);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
