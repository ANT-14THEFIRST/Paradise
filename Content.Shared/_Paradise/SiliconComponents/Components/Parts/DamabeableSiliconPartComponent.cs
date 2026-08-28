

using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]

public sealed partial class DamabeableSiliconPartComponent : Component //Yeah-yeah the naming is pretty messed up
{
    [DataField]
    public FixedPoint2 MaxDamageToRemainFunctional = 35;

    [DataField]
    public FixedPoint2 MinDamageToMalfunction = 20;

    [AutoNetworkedField]
    public FixedPoint2 CurrentDamageEfficiencyModifier = 1;
}
