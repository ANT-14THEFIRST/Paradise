using Content.Server.Traitor.Uplink;
using Content.Shared._Paradise.SiliconComponents;
using Content.Shared.Store.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Systems;

namespace Content.Server._Paradise.SiliconComponents;

public sealed partial class SiliconComponentsSystem : SharedSiliconComponentsSystem
{
    [Dependency] private UplinkSystem _uplink = default!;
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;

    public static readonly EntProtoId<StoreComponent> HiddenUplink = "SynthModHiddenUplink";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SiliconComponentsComponent, FallbackUplinkRequiredEvent>(OnFallbackUplinkRequest);
    }

    private void OnFallbackUplinkRequest(Entity<SiliconComponentsComponent> ent, ref FallbackUplinkRequiredEvent args)
    {
        if (ent.Comp.ModuleContainer == null)
            return;

        var uplinkEnt = Spawn(HiddenUplink, MapCoordinates.Nullspace);

        if (!_container.Insert(uplinkEnt, ent.Comp.ModuleContainer))
            return;

        _uplink.SetUplink(ent, uplinkEnt, args.Balance, args.GiveDiscounts);
        //if (_uplink.TryAddEntityUplink(ent, args.Balance, out var generatedCode, uplinkEnt, uplinkEnt, args.GiveDiscounts, false))
        args.Handled = true;
    }

    protected override void OnDamageChanged(Entity<SiliconComponentsComponent> ent, ref DamageDealtEvent args)
    {
        base.OnDamageChanged(ent, ref args);

        UpdateUserInterface(ent);
    }

    protected override void OnPartInserted(Entity<SiliconPartComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        base.OnPartInserted(ent, ref args);

        if (!TryComp<SiliconComponentsComponent>(args.Owner, out var siliconComp))
            return;

        UpdateUserInterface((args.Owner, siliconComp));
    }

    protected override void OnPartRemoved(Entity<SiliconPartComponent> ent, ref ComponentGotRemovedFromUser args)
    {
        base.OnPartRemoved(ent, ref args);

        if (!TryComp<SiliconComponentsComponent>(args.Owner, out var siliconComp))
            return;

        UpdateUserInterface((args.Owner, siliconComp));
    }

    public void UpdateUserInterface(Entity<SiliconComponentsComponent> ent)
    {
        var state = new SiliconBoundUiState();

        _ui.SetUiState(ent.Owner, SiliconUiKey.Key, state);
    }
}
