using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class GrownUpWear : OverallsItem
{
    internal override int Defense => 30;
    internal override int PowAdditive => 45;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 30);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}