using Content.Shared.AltBlocking;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.AltBlocking;

public sealed partial class AltBlockingIndicatorSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AltBlockingUserComponent, GetStatusIconsEvent>(OnGetStatusIcon);
    }

    private void OnGetStatusIcon(Entity<AltBlockingUserComponent> ent, ref GetStatusIconsEvent args)
    {
        if (_prototype.TryIndex(ent.Comp.Icon, out var iconPrototype) && ent.Comp.Blocking)
            args.StatusIcons.Add(iconPrototype);
    }
}
