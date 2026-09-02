using Robust.Shared.GameStates;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SiliconModuleComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public EntityUid? ModuleOwner = null;

    [DataField]
    public int OccupiedSpace = 1;

    [DataField]
    public TimeSpan TimeToInstall = new TimeSpan(0, 0, 5);
}
