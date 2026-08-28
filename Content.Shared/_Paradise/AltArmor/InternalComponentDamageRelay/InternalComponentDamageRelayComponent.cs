using Content.Shared._Paradise.AltArmor.Components;
using Content.Shared.Random;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Paradise.InternalComponentDamageRelay;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]

public sealed partial class InternalComponentDamageRelayComponent : AltArmorComponent
{
    [DataField]
    public ProtoId<WeightedRandomPrototype> Containers = string.Empty;

    [DataField]
    public bool ApplyNegative = false;
}
