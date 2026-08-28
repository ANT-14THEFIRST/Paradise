using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._Paradise.Jobs;

public sealed partial class AddEncryptionKeysSpecial : JobSpecial
{
    [DataField]
    public string KeySlot = "key_slots";

    [DataField(required: true)]
    public List<EntProtoId> Keys { get; private set; } = new();

    public override void AfterEquip(EntityUid mob)
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        var keyAdd = entMan.System<AddEncryptionKeysSpecialSystem>();
        keyAdd.SetupEncryptionKeys(mob, Keys, KeySlot);
    }
}
