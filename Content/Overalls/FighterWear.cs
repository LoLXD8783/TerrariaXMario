using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class FighterWear : OverallsItem
{
    internal override int Defense => 18;
    internal override int PowAdditive => 28;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}