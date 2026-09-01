using Content.Shared._Paradise.SiliconComponents;
using Content.Shared.Alert;
using Content.Shared.Damage.Systems;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._Paradise.SiliconComponents;

public sealed partial class SiliconComponentsSystem : SharedSiliconComponentsSystem
{
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private SharedBatterySystem _battery = default!;
    [Dependency] private AlertsSystem _alerts = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IPlayerManager _player = default!;

    private static readonly TimeSpan AlertUpdateDelay = TimeSpan.FromSeconds(0.5f);

    private TimeSpan _nextAlertUpdate = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SiliconComponentsComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<SiliconComponentsComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnPlayerAttached(Entity<SiliconComponentsComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        UpdateBatteryAlert((ent.Owner, ent.Comp, null));
    }

    private void OnPlayerDetached(Entity<SiliconComponentsComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        _alerts.ClearAlert(ent.Owner, ent.Comp.BatteryAlert);
        _alerts.ClearAlert(ent.Owner, ent.Comp.NoBatteryAlert);
    }

    protected override void OnDamageChanged(Entity<SiliconComponentsComponent> ent, ref DamageDealtEvent args)
    {
        base.OnDamageChanged(ent, ref args);
    }

    protected override void OnPartInserted(Entity<SiliconPartComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        base.OnPartInserted(ent, ref args);
    }

    protected override void OnPartRemoved(Entity<SiliconPartComponent> ent, ref ComponentGotRemovedFromUser args)
    {
        base.OnPartRemoved(ent, ref args);
    }

    private void UpdateBatteryAlert(Entity<SiliconComponentsComponent, PowerCellSlotComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp2, false))
            return;

        if (!_powerCell.TryGetBatteryFromSlot((ent.Owner, ent.Comp2), out var battery))
        {
            _alerts.ShowAlert(ent.Owner, ent.Comp1.NoBatteryAlert);
            return;
        }

        var chargeLevel = (short)MathF.Round(_battery.GetChargeLevel(battery.Value.AsNullable()) * 10f);

        if (chargeLevel == 0 && _powerCell.HasDrawCharge((ent.Owner, null, ent.Comp2)))
            chargeLevel = 1;

        _alerts.ShowAlert(ent.Owner, ent.Comp1.BatteryAlert, chargeLevel);
    }

    public override void UpdateUI(Entity<SiliconComponentsComponent?> ent)
    {
        if (_ui.TryGetOpenUi(ent.Owner, SiliconUiKey.Key, out var bui))
            bui.Update();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_player.LocalEntity is not { } localPlayer)
            return;

        var curTime = _timing.CurTime;

        if (curTime < _nextAlertUpdate)
            return;

        _nextAlertUpdate = curTime + AlertUpdateDelay;

        if (!TryComp<SiliconComponentsComponent>(localPlayer, out var owner) || !TryComp<PowerCellSlotComponent>(localPlayer, out var slot))
            return;

        UpdateBatteryAlert((localPlayer, owner, slot));
    }
}
