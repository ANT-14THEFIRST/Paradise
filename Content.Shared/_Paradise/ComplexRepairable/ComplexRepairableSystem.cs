using Content.Shared.Administration.Logs;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Repairable;
using Content.Shared.Stacks;
using Content.Shared.Tools.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared._Paradise.ComplexRepairable;

public sealed partial class ComplexRepairableSystem : EntitySystem
{
    [Dependency] private SharedToolSystem _toolSystem = default!;
    [Dependency] private DamageableSystem _damageableSystem = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private SharedStackSystem _stack = default!;

    private static readonly LocId MaterialRepair = "complex-repairable-material-repair";

    private static readonly LocId RepairDone = "comp-repairable-repair";

    private static readonly LocId NeedMoreMaterials = "comp-repairable-more-mats";

    public override void Initialize()
    {
        SubscribeLocalEvent<ComplexRepairableComponent, InteractUsingEvent>(Repair);
        SubscribeLocalEvent<ComplexRepairableComponent, ComplexRepairFinishedEvent>(OnRepairFinished);
        SubscribeLocalEvent<ComplexRepairableComponent, DamageDealtEvent>(OnDamageChanged);
    }

    private void OnDamageChanged(Entity<ComplexRepairableComponent> ent, ref DamageDealtEvent args)
    {
        var damageTaken = args.Damage?.GetTotal() ?? FixedPoint2.Zero;

        var threshold = ent.Comp.MaterialRepairThreshold;

        if (threshold == 0)
            threshold = 1;

        if (damageTaken > 0 && threshold != 0)
            ent.Comp.LeftToInsert += (damageTaken / threshold).Int();

        ent.Comp.DamageSinceLastThresholdUpdate += damageTaken.Float() % threshold.Float();//Offi govna poeli

        var toAdd = ent.Comp.DamageSinceLastThresholdUpdate / threshold;

        if (toAdd > 1)
        {
            ent.Comp.LeftToInsert += toAdd.Int();
            ent.Comp.DamageSinceLastThresholdUpdate -= toAdd.Int() * threshold;
        }

        Dirty(ent);
    }

    private void OnRepairFinished(Entity<ComplexRepairableComponent> ent,  ref ComplexRepairFinishedEvent args)
    {
        if (args.Cancelled)
            return;

        if (_damageableSystem.GetTotalDamage(ent.Owner) == 0)
            return;

        if (ent.Comp.DamageValue != FixedPoint2.Zero)
        {
            var damageChanged = _damageableSystem.HealEvenly(ent.Owner, ent.Comp.DamageValue, origin: args.User);
            _adminLogger.Add(LogType.Healed, $"{ToPrettyString(args.User):user} repaired {ToPrettyString(ent.Owner):target} by {damageChanged.GetTotal()}");
        }

        else
        {
            // Repair all damage
            _damageableSystem.SetAllDamage(ent.Owner, 0);
            _adminLogger.Add(LogType.Healed, $"{ToPrettyString(args.User):user} repaired {ToPrettyString(ent.Owner):target} back to full health");
        }

        var str = Loc.GetString(RepairDone, ("target", ent.Owner), ("tool", args.Used!));
        _popup.PopupClient(str, ent.Owner, args.User);

        var ev = new ComplexRepairedEvent(ent, args.User);
        RaiseLocalEvent(ent.Owner, ref ev);
        if (ent.Comp.AutoDoAfter &&
            args.Used is { Valid: true } usedValid)
        {
            float delay = ent.Comp.RepairTime;

            if (args.User == args.Target)
            {
                if (!ent.Comp.AllowSelfRepair)
                    return;

                delay *= ent.Comp.SelfRepairPenalty;
            }

            args.Handled = _toolSystem.UseTool(usedValid, args.User, ent.Owner, delay, ent.Comp.QualityNeeded, new ComplexRepairFinishedEvent(), ent.Comp.FuelCost.Float());
        }
    }

    private void Repair(Entity<ComplexRepairableComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        // Only try repair the target if it is damaged
        if (_damageableSystem.GetTotalDamage(ent.Owner) == 0)
            return;

        if (ent.Comp.LeftToInsert > 0)
        {
            if (MetaData(args.Used).EntityPrototype is not { } entityProto)
                return;

            if (ent.Comp.Material.Id != entityProto.ID)
                return;

            if (!TryComp<StackComponent>(args.Used, out var stackComp))
            {
                QueueDel(args.Used);
                ent.Comp.LeftToInsert -= 1;
            }

            if (stackComp != null)
            {
                int toBeUsed = ent.Comp.LeftToInsert;

                if (stackComp.Count < ent.Comp.LeftToInsert)
                    toBeUsed = stackComp.Count;

                var str = Loc.GetString(MaterialRepair, ("target", ent.Owner), ("material", args.Used!));
                _popup.PopupClient(str, ent.Owner, args.User);

                _stack.TryUse(args.Used, toBeUsed);

                ent.Comp.LeftToInsert -= toBeUsed;
            }

            args.Handled = true;

            Dirty(ent);
            return;
        }

        if (ent.Comp.MaterialRepairThreshold * ent.Comp.LeftToInsert > _damageableSystem.GetTotalDamage(ent.Owner) - ent.Comp.DamageValue)
        {
            var str = Loc.GetString(NeedMoreMaterials);
            _popup.PopupEntity(str, ent.Owner, args.User);
            return;
        }

        if (ent.Comp.LeftToInsert > 0)
        {
            Dirty(ent);
            return;
        }

        float delay = ent.Comp.RepairTime;

        // Add a penalty to how long it takes if the user is repairing itself
        if (args.User == args.Target)
        {
            if (!ent.Comp.AllowSelfRepair)
                return;

            delay *= ent.Comp.SelfRepairPenalty;
        }

        // Run the repairing doafter
        args.Handled = _toolSystem.UseTool(args.Used, args.User, ent.Owner, delay, ent.Comp.QualityNeeded, new ComplexRepairFinishedEvent(), ent.Comp.FuelCost.Float());
    }
}

/// <summary>
/// Event raised on an entity when its successfully repaired.
/// </summary>
/// <param name="Ent"></param>
/// <param name="User"></param>
[ByRefEvent]
public readonly record struct ComplexRepairedEvent(Entity<ComplexRepairableComponent> Ent, EntityUid User);

[Serializable, NetSerializable]
public sealed partial class ComplexRepairFinishedEvent : SimpleDoAfterEvent;
