using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared.ToggleBlocking;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ToggleBlockingChanceComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public DamageModifierSet? OriginalActiveModifier;

    [DataField, AutoNetworkedField]
    public bool Toggled = false;

    [DataField, AutoNetworkedField]
    public float ToggledRangeBlockProb = 0.5f;

    [DataField, AutoNetworkedField]
    public float BaseRangeBlockProb = 0f;

    [DataField, AutoNetworkedField]
    public float ToggledMeleeBlockProb = 0.5f;

    [DataField, AutoNetworkedField]
    public float BaseMeleeBlockProb = 0f;
}
