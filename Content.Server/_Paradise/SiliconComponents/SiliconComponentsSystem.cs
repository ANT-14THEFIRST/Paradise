using Content.Server.Traitor.Uplink;
using Content.Shared._Paradise.SiliconComponents;
using Content.Shared.Store.Components;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server._Paradise.SiliconComponents;

public sealed partial class SiliconComponentsSystem : SharedSiliconComponentsSystem
{
    [Dependency] private UplinkSystem _uplink = default!;
    [Dependency] private SharedContainerSystem _container = default!;

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
}
