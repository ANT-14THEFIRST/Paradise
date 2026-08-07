using Robust.Shared.GameStates;

namespace Content.Shared.ArmorBlock;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]

public sealed partial class ArmorBlockComponent : Component
{
    /// <summary>
    /// The entity this armor protects(must be set manually in every implementation, made for reusability)
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? User = null;
}
