using Content.Shared.EntityEffects.Effects.StatusEffects;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
public sealed partial class MovementSpeedModifyingModuleComponent : Component
{
    [DataField]
    public bool RequiresToggle = false;

    [DataField]
    public bool ReverseToggle = false;

    [DataField]
    public MovementSpeedModifier SpeedMod = new MovementSpeedModifier();
}
