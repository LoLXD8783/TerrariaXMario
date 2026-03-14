using Terraria.UI;

namespace TerrariaXMario.Common.UIElements;

internal class BetterUIState : UIState
{
    internal static Player Player => Main.LocalPlayer;
    internal virtual bool Visible => true;
    internal virtual InterfaceScaleType InterfaceScaleType => InterfaceScaleType.UI;
}
