

using Robust.Shared.GameStates;
using Content.Shared.Whitelist;

namespace Content.Shared._Paradise.SiliconComponents;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SiliconModuleBlacklistComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public EntityWhitelist? ModuleBlacklist;
}
