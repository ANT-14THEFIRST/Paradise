using Content.Shared.Actions.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SiliconModuleToggleableComponent : Component
{
    [DataField(required: true)]
    public EntProtoId<InstantActionComponent> Action;

    [DataField, AutoNetworkedField]
    public EntityUid? ActionEntity;

    [DataField]
    public bool DisableOnUnequip;
}
