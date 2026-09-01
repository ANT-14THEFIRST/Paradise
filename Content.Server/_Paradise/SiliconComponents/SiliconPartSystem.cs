using Content.Shared._Paradise.SiliconComponents;

namespace Content.Server._Paradise.SiliconComponents;

public sealed partial class SiliconPartSystem : SharedSiliconPartSystem
{
    [Dependency] private SiliconComponentsSystem _silicon = default!;

    protected override void UpdateDamageStatus(Entity<DamabeableSiliconPartComponent> ent)
    {
        base.UpdateDamageStatus(ent);

        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp) ||
            partComp.PartOwner is not { Valid: true } ownerValid ||
            !TryComp<SiliconComponentsComponent>(partComp.PartOwner, out var siliconComp))
            return;

        _silicon.UpdateUserInterface((ownerValid, siliconComp));
    }
}


