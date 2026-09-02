using Content.Shared.Alert;
using Content.Shared.Body.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._Paradise.SiliconComponents;

public abstract partial class SharedSiliconPartSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageableSystem = default!;
    [Dependency] private BlindableSystem _blindable = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedMindSystem _mind = default!;
    [Dependency] private ISharedPlayerManager _player = default!;
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private MovementSpeedModifierSystem _movement = default!;
    [Dependency] private SharedSiliconComponentsSystem _siliconComponents = default!;
    [Dependency] private AlertsSystem _alerts = default!;

    private static readonly string PartContainerPrefix = "silicon_component";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamabeableSiliconPartComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<DamabeableSiliconPartComponent, ComponentShutdown>(OnComponentShutdown);

        SubscribeLocalEvent<SiliconComponentsComponent, CanSeeAttemptEvent>(OnCanSeeCheck);

        SubscribeLocalEvent<ActiveOpticsComponent, ComponentGotInsertedIntoUser>(OnOpticsInserted);
        SubscribeLocalEvent<ActiveOpticsComponent, ComponentGotRemovedFromUser>(OnOpticsRemoved);

        SubscribeLocalEvent<BrainComponent, ComponentGotInsertedIntoUser>(OnBrainInserted);
        SubscribeLocalEvent<BrainComponent, ComponentGotRemovedFromUser>(OnBrainRemoved);

        SubscribeLocalEvent<ActiveOpticsComponent, SiliconPartStatusOnline>(OnPartOnline);
        SubscribeLocalEvent<ActiveOpticsComponent, SiliconPartStatusOffline>(OnPartOffline);

        SubscribeLocalEvent<ActiveOpticsComponent, SiliconPartDamageModifierChanged>(OnPartDamageModChanged);

        SubscribeLocalEvent<MovementSpeedModifyingPartComponent, SiliconPartStatusOnline>(OnMovementModifierOnline);
        SubscribeLocalEvent<MovementSpeedModifyingPartComponent, SiliconPartStatusOffline>(OnMovementModifierOffline);

        SubscribeLocalEvent<MovementSpeedModifyingPartComponent, ComponentGotInsertedIntoUser>(OnMovementModifierInserted);
        SubscribeLocalEvent<MovementSpeedModifyingPartComponent, ComponentGotRemovedFromUser>(OnMovementModifierRemoved);

        SubscribeLocalEvent<SiliconPartComponent, MindAddedMessage>(OnBrainMindAdded);

        SubscribeLocalEvent<DamabeableSiliconPartComponent, DamageDealtEvent>(OnDamageChanged);

        SubscribeLocalEvent<DamabeableSiliconPartComponent, ComponentGotInsertedIntoUser>(OnDamageableInserted);
    }

    private void OnComponentStartup(Entity<DamabeableSiliconPartComponent> ent, ref ComponentStartup args)
    {
        UpdateDamageStatus(ent);
    }

    private void OnComponentShutdown(Entity<DamabeableSiliconPartComponent> ent, ref ComponentShutdown args)
    {
        UpdateDamageStatus(ent);
    }

    private void OnDamageChanged(Entity<DamabeableSiliconPartComponent> ent, ref DamageDealtEvent args)
    {
        UpdateDamageStatus(ent);
    }

    private void OnDamageableInserted(Entity<DamabeableSiliconPartComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        RefreshAlerts(args.Owner);
    }

    protected virtual void UpdateDamageStatus(Entity<DamabeableSiliconPartComponent> ent)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp))
            return;

        if (TryGetIntegrityModifier(ent.AsNullable(), out FixedPoint2 modifier) &&
            ent.Comp.CurrentDamageEfficiencyModifier != modifier)
        {
            ent.Comp.CurrentDamageEfficiencyModifier = modifier;

            var damageModEvent = new SiliconPartDamageModifierChanged(modifier);
            RaiseLocalEvent(ent.Owner, ref damageModEvent);
        }

        if (_damageableSystem.GetTotalDamage(ent.Owner) > ent.Comp.MaxDamageToRemainFunctional && partComp.Active)
        {
            partComp.Active = false;

            var offlineEv = new SiliconPartStatusOffline(partComp.PartOwner);
            RaiseLocalEvent(ent.Owner, ref offlineEv);

            Dirty(ent);

            return;
        }

        if (_damageableSystem.GetTotalDamage(ent.Owner) < ent.Comp.MaxDamageToRemainFunctional && !partComp.Active)
        {
            partComp.Active = true;

            var onlineEv = new SiliconPartStatusOnline(partComp.PartOwner);
            RaiseLocalEvent(ent.Owner, ref onlineEv);
        }

        Dirty(ent);
    }

    private void OnCanSeeCheck(Entity<SiliconComponentsComponent> ent, ref CanSeeAttemptEvent args)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        if (!ent.Comp.Online)
        {
            args.Cancel();
            return;
        }

        if (!ent.Comp.Parts.TryGetValue(PartType.Optics, out var opticsContainer) || opticsContainer.ContainedEntity is not { Valid: true } opticsValidated)
        {
            args.Cancel();
            return;
        }

        if (!HasComp<ActiveOpticsComponent>(opticsValidated) || TryComp<SiliconPartComponent>(ent.Owner, out var partComp) && !partComp.Active)
        {
            args.Cancel();
            return;
        }
    }

    private void OnMovementModifierInserted(Entity<MovementSpeedModifyingPartComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        _movement.RefreshMovementSpeedModifiers(ent.Owner);
    }

    private void OnMovementModifierRemoved(Entity<MovementSpeedModifyingPartComponent> ent, ref ComponentGotRemovedFromUser args)
    {
        _movement.RefreshMovementSpeedModifiers(ent.Owner);
    }

    private void OnMovementModifierOnline(Entity<MovementSpeedModifyingPartComponent> ent, ref SiliconPartStatusOnline args)
    {
        _movement.RefreshMovementSpeedModifiers(ent.Owner);
    }

    private void OnMovementModifierOffline(Entity<MovementSpeedModifyingPartComponent> ent, ref SiliconPartStatusOffline args)
    {
        _movement.RefreshMovementSpeedModifiers(ent.Owner);
    }

    private void OnBrainInserted(Entity<BrainComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        if (TerminatingOrDeleted(ent) || TerminatingOrDeleted(args.Owner))
            return;

        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp))
            return;

        if (!_container.TryGetContainingContainer(ent.Owner, out var container))
            return;

        if (partComp.PartOwner is not { Valid: true } ownerValidated)
            return;

        if (!TryComp<SiliconComponentsComponent>(ownerValidated, out var siliconComp) ||
            container.ID != PartContainerPrefix + "_" + PartType.Brain)
            return;

        if (_mind.TryGetMind(ent.Owner, out var mindId, out var mind) &&
            _player.TryGetSessionById(mind.UserId, out var session))
            _mind.TransferTo(mindId, ownerValidated, mind: mind);

    }

    private void OnBrainRemoved(Entity<BrainComponent> ent, ref ComponentGotRemovedFromUser args)
    {
        if (TerminatingOrDeleted(ent) || TerminatingOrDeleted(args.Owner))
            return;

        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp))
            return;

        if (_mind.TryGetMind(args.Owner, out var mindId, out var mind))
            _mind.TransferTo(mindId, ent.Owner, mind: mind);
    }

    private void OnBrainMindAdded(Entity<SiliconPartComponent> ent, ref MindAddedMessage args)
    {
        if (!_container.TryGetContainingContainer(ent.Owner, out var container))
            return;

        if (ent.Comp.PartOwner is not { Valid: true } ownerValidated)
            return;

        if (!TryComp<SiliconComponentsComponent>(ownerValidated, out var siliconComp) ||
            container.ID != PartContainerPrefix + "_" + PartType.Brain)
            return;

        if (_mind.TryGetMind(ent.Owner, out var mindId, out var mind) &&
            _player.TryGetSessionById(mind.UserId, out var session))
            _mind.TransferTo(mindId, ownerValidated, mind: mind);
    }

    private void OnOpticsInserted(Entity<ActiveOpticsComponent> ent, ref ComponentGotInsertedIntoUser args)
    {
        if (!HasComp<SiliconComponentsComponent>(args.Owner))
            return;

        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) || args.Owner is not { Valid: true })
            return;

        if (TryComp<DamabeableSiliconPartComponent>(ent.Owner, out var damageablePartComp))
            if (TryComp<BlindableComponent>(args.Owner, out var ownerBlindableComp))
                _blindable.AdjustEyeDamage(args.Owner, (ownerBlindableComp.MaxDamage * damageablePartComp.CurrentDamageEfficiencyModifier).Int() - ownerBlindableComp.EyeDamage);

        _blindable.UpdateIsBlind(args.Owner);
    }

    private void OnOpticsRemoved(Entity<ActiveOpticsComponent> ent, ref ComponentGotRemovedFromUser args)
    {
        if (!TryComp<SiliconComponentsComponent>(args.Owner, out var ownerComp))
            return;

        if (TryComp<BlindableComponent>(args.Owner, out var ownerBlindableComp))
            _blindable.AdjustEyeDamage(args.Owner, -ownerBlindableComp.EyeDamage);

        _blindable.UpdateIsBlind(args.Owner);
    }

    private void OnPartOnline(Entity<ActiveOpticsComponent> ent, ref SiliconPartStatusOnline args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) || partComp.PartOwner is not { Valid: true } ownerValidated)
            return;

        _blindable.UpdateIsBlind(ownerValidated);
    }

    private void OnPartOffline(Entity<ActiveOpticsComponent> ent, ref SiliconPartStatusOffline args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) || partComp.PartOwner is not { Valid: true } ownerValidated)
            return;

        _blindable.UpdateIsBlind(ownerValidated);
    }

    private void OnPartDamageModChanged(Entity<ActiveOpticsComponent> ent, ref SiliconPartDamageModifierChanged args)
    {
        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) || partComp.PartOwner is not { Valid: true } ownerValidated)
            return;

        if (!TryComp<BlindableComponent>(ownerValidated, out var blindableComp))
            return;

        _blindable.AdjustEyeDamage(ownerValidated, (blindableComp.MaxDamage * args.Modifier).Int() - blindableComp.EyeDamage);

        _blindable.UpdateIsBlind(ownerValidated);
    }

    public bool TryGetIntegrityModifier(Entity<DamabeableSiliconPartComponent?> part, out FixedPoint2 modifier)
    {
        modifier = 1;

        if (!Resolve(part.Owner, ref part.Comp))
            return false;

        modifier = FixedPoint2.Clamp(
            (_damageableSystem.GetTotalDamage(part.Owner) - part.Comp.MinDamageToMalfunction) /
            (part.Comp.MaxDamageToRemainFunctional - part.Comp.MinDamageToMalfunction),
            0,
            1);

        return true;
    }

    public void RefreshAlerts(Entity<SiliconComponentsComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp))
            return;

        short severity = -1;

        foreach (var part in ent.Comp.Parts.Keys)
        {
            if (!_siliconComponents.TryGetPart(ent, part, out var partUid) ||
                partUid is not { Valid: true } partValidated ||
                !TryComp<DamabeableSiliconPartComponent>(partValidated, out var partDamageableComp))
                continue;

            if (_damageableSystem.GetTotalDamage(partValidated) >= partDamageableComp.MinDamageToMalfunction)
                severity += 1;
        }

        if (severity == -1)
        {
            _alerts.ClearAlert(ent.Owner, ent.Comp.MalfunctionAlertProto);
            return;
        }

        _alerts.UpdateAlert(ent.Owner, ent.Comp.MalfunctionAlertProto, severity: severity);
    }

    public void SetOperational(Entity<SiliconComponentsComponent?> ent, bool operational)
    {
        if (!Resolve(ent.Owner, ref ent.Comp))
            return;

        if (operational)
        {
            RemComp<StunnedComponent>(ent);
            RemComp<KnockedDownComponent>(ent);
            return;
        }

        EnsureComp<StunnedComponent>(ent);
        EnsureComp<KnockedDownComponent>(ent);
    }
}

[ByRefEvent]
public record struct SiliconPartStatusOnline(EntityUid? Owner)
{
}

[ByRefEvent]
public record struct SiliconPartStatusOffline(EntityUid? Owner)
{
}

[ByRefEvent]
public record struct SiliconPartDamageModifierChanged(FixedPoint2 Modifier)
{
}

