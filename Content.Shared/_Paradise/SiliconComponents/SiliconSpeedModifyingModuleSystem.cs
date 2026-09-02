using Content.Shared.Actions;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Toggleable;

namespace Content.Shared._Paradise.SiliconComponents;

public sealed partial class SiliconSpeedModifyingModuleSystem : EntitySystem
{
    [Dependency] private SharedActionsSystem _actions = default!;
    [Dependency] private ItemToggleSystem _toggle = default!;
    [Dependency] private MovementSpeedModifierSystem _movement = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MovementSpeedModifyingModuleComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<MovementSpeedModifyingModuleComponent, ComponentShutdown>(OnComponentShutdown);

        SubscribeLocalEvent<MovementSpeedModifyingModuleComponent, ItemToggledEvent>(OnModuleToggled);

        SubscribeLocalEvent<MovementSpeedModifyingModuleComponent, SiliconModuleGotInserted>(OnModuleInserted);
        SubscribeLocalEvent<MovementSpeedModifyingModuleComponent, SiliconModuleGotRemoved>(OnModuleRemoved);
    }

    private void OnComponentStartup(Entity<MovementSpeedModifyingModuleComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<SiliconModuleComponent>(ent.Owner, out var moduleComp) ||
            moduleComp.ModuleOwner is not { Valid: true } ownerValidated)
            return;

        _movement.RefreshMovementSpeedModifiers(ownerValidated);
    }

    private void OnComponentShutdown(Entity<MovementSpeedModifyingModuleComponent> ent, ref ComponentShutdown args)
    {
        if (!TryComp<SiliconModuleComponent>(ent.Owner, out var moduleComp) ||
            moduleComp.ModuleOwner is not { Valid: true } ownerValidated)
            return;

        _movement.RefreshMovementSpeedModifiers(ownerValidated);
    }

    private void OnModuleInserted(Entity<MovementSpeedModifyingModuleComponent> ent, ref SiliconModuleGotInserted args)
    {
        _movement.RefreshMovementSpeedModifiers(args.Owner);
    }

    private void OnModuleRemoved(Entity<MovementSpeedModifyingModuleComponent> ent, ref SiliconModuleGotRemoved args)
    {
        _movement.RefreshMovementSpeedModifiers(args.Owner);
    }

    private void OnModuleToggled(Entity<MovementSpeedModifyingModuleComponent> ent, ref ItemToggledEvent args)
    {
        if (!TryComp<SiliconModuleComponent>(ent.Owner, out var moduleComp) ||
            moduleComp.ModuleOwner is not { Valid: true } ownerValidated)
            return;

        _movement.RefreshMovementSpeedModifiers(ownerValidated);
    }
}
