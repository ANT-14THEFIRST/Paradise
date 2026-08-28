

using Robust.Shared.GameStates;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]

public sealed partial class SiliconPartComponent : Component //Yeah-yeah the naming is pretty messed up
{
    [DataField]
    [AutoNetworkedField]
    public bool Active = true;

    [DataField]
    [AutoNetworkedField]
    public EntityUid? PartOwner = null;

    [DataField]
    public PartType PartType;

    [DataField]
    public int OccupiedSpace = 1;

    [DataField]
    public TimeSpan TimeToInstall = new TimeSpan(0, 0, 5);
}
