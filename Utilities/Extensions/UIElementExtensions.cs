using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace TerrariaXMario.Utilities.Extensions;

internal static class UIElementExtensions
{
    extension<T>(T element) where T : UIElement
    {
        public T With(Action<T> action)
        {
            action(element);
            return element;
        }
    }

    extension(UIElement parent)
    {
        public T AddElement<T>(T child) where T : UIElement
        {
            if (parent is UIGrid uiGrid) uiGrid.Add(child);
            else if (parent is UIList uiList) uiList.Add(child);
            else parent.Append(child);

            return child;
        }
    }
}
