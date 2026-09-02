namespace Content.Shared.Weapons.Melee.Events;

using Content.Shared.Damage;

[ByRefEvent]
public record struct MeleeAttackerEvent(EntityUid Used, EntityUid Target, DamageSpecifier Damage)
{
    public EntityUid Used = Used;

    public EntityUid Target = Target;

    public DamageSpecifier Damage = Damage;

    public DamageSpecifier ModifiedDamage = new DamageSpecifier();
}
