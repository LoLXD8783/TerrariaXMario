using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class MuscleWear : OverallsItem
{
    internal override int Defense => 54;
    internal override int PowAdditive => 70;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(1, 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
