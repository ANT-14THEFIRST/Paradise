using Content.Shared.Item.ItemToggle.Components;

namespace Content.Shared._Paradise.SiliconComponents;

public sealed partial class ComponentAddingSiliconComponentSystem : EntitySystem
{
    [Dependency] private IEntityManager _entManager = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ComponentAddingSiliconPartComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<ComponentAddingSiliconPartComponent, ComponentShutdown>(OnComponentShutdown);

        SubscribeLocalEvent<ComponentAddingSiliconPartComponent, ComponentGotInsertedIntoUser>(OnEntityInserted);
        SubscribeLocalEvent<ComponentAddingSiliconPartComponent, ComponentGotRemovedFromUser>(OnEntityRemoved);

        SubscribeLocalEvent<ComponentAddingSiliconPartComponent, SiliconPartStatusOnline>(OnPartOnline);
        SubscribeLocalEvent<ComponentAddingSiliconPartComponent, SiliconPartStatusOffline>(OnPartOffline);

        SubscribeLocalEvent<ComponentAddingSiliconModuleComponent, ComponentStartup>(OnComponentStartupModule);
        SubscribeLocalEvent<ComponentAddingSiliconModuleComponent, ComponentShutdown>(OnComponentShutdownModule);

        SubscribeLocalEvent<ComponentAddingSiliconModuleComponent, SiliconModuleGotInserted>(OnModuleInserted);
        SubscribeLocalEvent<ComponentAddingSiliconModuleComponent, SiliconModuleGotRemoved>(OnModuleRemoved);

        SubscribeLocalEvent<ComponentAddingSiliconModuleComponent, ItemToggledEvent>(OnModuleToggled);
    }

    private void OnEntityInserted(Entity<ComponentAddingSiliconPartComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        if (!HasComp<SiliconComponentsComponent>(args.Owner))
            return;

        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) ||
            partComp.PartOwner is not { Valid: true } ||
            !partComp.Active)
            return;

        _entManager.AddComponents(args.Owner, ent.Comp.Components);
    }

    private void OnEntityRemoved(Entity<ComponentAddingSiliconPartComponent> ent, ref ComponentGotRemovedFromUser args)
    {
        if (!TryComp<SiliconComponentsComponent>(args.Owner, out var ownerComp))
            return;

        _entManager.RemoveComponents(args.Owner, ent.Comp.Components);
    }

    private void OnComponentStartup(Entity<ComponentAddingSiliconPartComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) ||
            partComp.PartOwner is not { Valid: true } ownerValidated ||
            !partComp.Active)
            return;

        if (!HasComp<SiliconComponentsComponent>(partComp.PartOwner))
            return;

        _entManager.AddComponents(ownerValidated, ent.Comp.Components);
    }

    private void OnComponentShutdown(Entity<ComponentAddingSiliconPartComponent> ent, ref ComponentShutdown args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) ||
            partComp.PartOwner is not { Valid: true } ownerValidated ||
            !partComp.Active)
            return;

        if (!HasComp<SiliconComponentsComponent>(partComp.PartOwner))
            return;

        _entManager.RemoveComponents(ownerValidated, ent.Comp.Components);
    }

    private void OnPartOnline(Entity<ComponentAddingSiliconPartComponent> ent, ref SiliconPartStatusOnline args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) ||
            partComp.PartOwner is not { Valid: true } ownerValidated ||
            !partComp.Active)
            return;

        if (!HasComp<SiliconComponentsComponent>(partComp.PartOwner))
            return;

        _entManager.AddComponents(ownerValidated, ent.Comp.Components);
    }

    private void OnPartOffline(Entity<ComponentAddingSiliconPartComponent> ent, ref SiliconPartStatusOffline args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) ||
            partComp.PartOwner is not { Valid: true } ownerValidated ||
            !partComp.Active)
            return;

        if (!HasComp<SiliconComponentsComponent>(partComp.PartOwner))
            return;

        _entManager.RemoveComponents(ownerValidated, ent.Comp.Components);
    }

    private void OnComponentStartupModule(Entity<ComponentAddingSiliconModuleComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<SiliconModuleComponent>(ent.Owner, out var moduleComp) ||
            moduleComp.ModuleOwner is not { Valid: true } ownerValidated)
            return;

        if (!HasComp<SiliconComponentsComponent>(moduleComp.ModuleOwner))
            return;

        _entManager.AddComponents(ownerValidated, ent.Comp.Components);
    }

    private void OnComponentShutdownModule(Entity<ComponentAddingSiliconModuleComponent> ent, ref ComponentShutdown args)
    {
        if (!TryComp<SiliconModuleComponent>(ent.Owner, out var moduleComp) ||
            moduleComp.ModuleOwner is not { Valid: true } ownerValidated)
            return;

        if (!HasComp<SiliconComponentsComponent>(moduleComp.ModuleOwner))
            return;

        _entManager.RemoveComponents(ownerValidated, ent.Comp.Components);
    }

    private void OnModuleInserted(Entity<ComponentAddingSiliconModuleComponent> ent, ref SiliconModuleGotInserted args)
    {
        if (!HasComp<SiliconComponentsComponent>(args.Owner))
            return;

        if (!ent.Comp.RequiresToggle)
        {
            _entManager.AddComponents(args.Owner, ent.Comp.Components);
            return;
        }

        if (!TryComp<ItemToggleComponent>(ent.Owner, out var toggleComp) ||
            ent.Comp.ReverseToggle == toggleComp.Activated)
            return;

        _entManager.AddComponents(args.Owner, ent.Comp.Components);
    }

    private void OnModuleRemoved(Entity<ComponentAddingSiliconModuleComponent> ent, ref SiliconModuleGotRemoved args)
    {
        if (!TryComp<SiliconComponentsComponent>(args.Owner, out var ownerComp))
            return;

        _entManager.RemoveComponents(args.Owner, ent.Comp.Components);
    }

    private void OnModuleToggled(Entity<ComponentAddingSiliconModuleComponent> ent, ref ItemToggledEvent args)
    {
        if (!ent.Comp.RequiresToggle)
            return;

        if (!TryComp<SiliconModuleComponent>(ent.Owner, out var moduleComp) ||
            moduleComp.ModuleOwner is not { Valid: true } ownerValidated)
            return;

        if (ent.Comp.ReverseToggle != args.Activated)
        {
            _entManager.AddComponents(ownerValidated, ent.Comp.Components);
            return;
        }

        _entManager.RemoveComponents(ownerValidated, ent.Comp.Components);
        return;
    }
}
