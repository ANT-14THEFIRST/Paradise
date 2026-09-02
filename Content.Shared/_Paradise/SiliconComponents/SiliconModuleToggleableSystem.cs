using Content.Shared.Actions;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Toggleable;

namespace Content.Shared._Paradise.SiliconComponents;

public sealed partial class SiliconModuleToggleableSystem : EntitySystem
{
    [Dependency] private SharedActionsSystem _actions = default!;
    [Dependency] private ItemToggleSystem _toggle = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SiliconModuleToggleableComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<SiliconModuleToggleableComponent, ToggleActionEvent>(OnToggleAction);

        SubscribeLocalEvent<SiliconModuleToggleableComponent, SiliconModuleGotInserted>(OnToggleableInstalled);
        SubscribeLocalEvent<SiliconModuleToggleableComponent, SiliconModuleGotRemoved>(OnToggleableRemoved);
    }

    private void OnMapInit(Entity<SiliconModuleToggleableComponent> ent, ref MapInitEvent args)
    {
        var (uid, comp) = ent;
        // test funny
        if (string.IsNullOrEmpty(comp.Action))
            return;

        _actions.AddAction(uid, ref comp.ActionEntity, comp.Action);
        _actions.SetToggled(comp.ActionEntity, _toggle.IsActivated(ent.Owner));
        Dirty(uid, comp);
    }

    private void OnToggleAction(Entity<SiliconModuleToggleableComponent> ent, ref ToggleActionEvent args)
    {
        args.Handled = _toggle.Toggle(ent.Owner, args.Performer);
    }

    private void OnToggleableInstalled(Entity<SiliconModuleToggleableComponent> ent, ref SiliconModuleGotInserted args)
    {
        _actions.AddAction(args.Owner, ref ent.Comp.ActionEntity, ent.Comp.Action, container: ent.Owner);
    }

    private void OnToggleableRemoved(Entity<SiliconModuleToggleableComponent> ent, ref SiliconModuleGotRemoved args)
    {
        _actions.RemoveAction(ent.Comp.ActionEntity);

        if (ent.Comp.DisableOnUnequip)
            _toggle.TryDeactivate(ent.Owner, args.Owner);
    }
}
