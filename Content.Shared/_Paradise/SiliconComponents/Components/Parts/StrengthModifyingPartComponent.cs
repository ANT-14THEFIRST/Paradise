

using Content.Shared.FixedPoint;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
public sealed partial class StrengthModifyingPartComponent : Component
{
    [DataField]
    public FixedPoint2 StrengthValue = 1.3;
}
