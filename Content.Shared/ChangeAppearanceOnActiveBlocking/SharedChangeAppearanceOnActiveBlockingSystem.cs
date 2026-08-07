using Content.Shared.AltBlocking;
using Robust.Shared.Serialization;

namespace Content.Shared.ChangeAppearanceOnActiveBlocking;

public sealed partial class SharedChangeAppearanceOnActiveBlockingSystem : EntitySystem
{
    [Dependency] private SharedAppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChangeAppearanceOnActiveBlockingComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<ChangeAppearanceOnActiveBlockingComponent, ActiveBlockingStateChanged>(OnActiveBlock);
    }

    private void OnInit(Entity<ChangeAppearanceOnActiveBlockingComponent> ent, ref ComponentInit args)
    {
        UpdateVisuals(ent);
        Dirty(ent, ent.Comp);
    }

    public void UpdateVisuals(Entity<ChangeAppearanceOnActiveBlockingComponent> ent)
    {
        _appearanceSystem.SetData(ent, ActiveBlockingVisuals.Enabled, ent.Comp.Toggled);
    }

    public void OnActiveBlock(Entity<ChangeAppearanceOnActiveBlockingComponent> ent, ref ActiveBlockingStateChanged args)
    {
        ent.Comp.Toggled = args.State;
        Dirty(ent);
        UpdateVisuals((ent.Owner, ent.Comp));
    }
}

[Serializable, NetSerializable]
public enum ActiveBlockingVisuals : byte
{
    Enabled,
    Layer,
    Color,
}
