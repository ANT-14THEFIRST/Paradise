using Content.Shared._Paradise.AltArmor;
using Content.Shared.Damage.Systems;
using Content.Shared.Random.Helpers;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._Paradise.InternalComponentDamageRelay;

public sealed partial class InternalComponentDamageRelaySystem : AltArmorSystem<InternalComponentDamageRelayComponent>
{
    [Dependency] private DamageableSystem _damageable = default!;
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<InternalComponentDamageRelayComponent, DamageModifyEvent>(OnDamageChange);
    }

    public void OnDamageChange(Entity<InternalComponentDamageRelayComponent> ent, ref DamageModifyEvent args)
    {
        ModifyDamage(ent.Owner, args.OriginalDamage, out var resultDamage, out var resultArmorDamage);

        args.Damage = resultArmorDamage;

        if (ent.Comp.Containers == string.Empty)
            return;

        if (!args.OriginalDamage.AnyPositive() &&
            !ent.Comp.ApplyNegative)
            return;

        var containerID = _prototype.Index(ent.Comp.Containers).Pick(_random);

        if (containerID == "None")
            return;

        if (!_container.TryGetContainer(ent.Owner, containerID, out var container) ||
            container is not ContainerSlot containerSlot ||
            containerSlot.ContainedEntity is not { Valid: true } internalComponent)
            return;

        _damageable.TryChangeDamage(internalComponent, resultDamage);
    }
}
