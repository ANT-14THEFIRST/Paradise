

using Content.Shared.EntityEffects.Effects.StatusEffects;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
public sealed partial class MovementSpeedModifyingPartComponent : Component
{
    [DataField]
    public bool RequiresActive = true;

    [DataField]
    public MovementSpeedModifier SpeedMod = new MovementSpeedModifier();
}
