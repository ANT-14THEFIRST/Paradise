

using Robust.Shared.Prototypes;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
public sealed partial class ComponentAddingSiliconPartComponent : Component //This will remove all components it adds on removal/destruction, do not use to modify existing components
{
    [DataField]
    public ComponentRegistry Components = new();
}
