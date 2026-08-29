using Content.Shared._Paradise.AltArmor;
using Content.Shared.Damage.Systems;

namespace Content.Shared._Paradise.ArmorBlock;

public sealed partial class ArmorBlockSystem : AltArmorSystem<ArmorBlockComponent>
{
    [Dependency] private DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ArmorBlockComponent, DamageModifyEvent>(OnDamageChange);
    }

    public void OnDamageChange(Entity<ArmorBlockComponent> ent, ref DamageModifyEvent args)
    {
        ModifyDamage(ent.Owner, args.OriginalDamage, out var resultDamage, out var resultArmorDamage);

        args.Damage = resultArmorDamage;

        if (ent.Comp.User == null)
            return;

        _damageable.TryChangeDamage((EntityUid)ent.Comp.User, resultDamage);
    }
}
