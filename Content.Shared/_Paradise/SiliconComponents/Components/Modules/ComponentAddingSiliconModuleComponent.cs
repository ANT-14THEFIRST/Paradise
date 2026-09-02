using Robust.Shared.Prototypes;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
public sealed partial class ComponentAddingSiliconModuleComponent : Component //This will remove all components it adds on removal, do not use to modify existing components
{
    [DataField]
    public ComponentRegistry Components = new();

    [DataField]
    public bool RequiresToggle = false;

    [DataField]
    public bool ReverseToggle = false;
}
