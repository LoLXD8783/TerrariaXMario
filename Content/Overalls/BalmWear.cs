using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class BalmWear : OverallsItem
{
    internal override int Defense => 45;
    internal override int PowAdditive => 60;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 95);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
