using Content.Shared.Alert;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.AltBlocking;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]

public sealed partial class AltBlockingUserComponent : Component
{
    /// <summary>
    /// The entities that's being used to block and are shields
    /// </summary>
    [AutoNetworkedField]
    public List<EntityUid> BlockingItemsShields = new();

    [DataField, AutoNetworkedField]
    public bool Blocking = false;

    [DataField]
    public ProtoId<AlertPrototype> BlockingAlertProtoId = "ActiveBlocking";

    [DataField]
    public ProtoId<BlockingIconPrototype> Icon = "BlockingIcon";
}
