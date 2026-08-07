using Robust.Shared.Serialization;

namespace Content.Shared.AltBlocking;

[Serializable, NetSerializable]
public sealed class BlockAttemptEvent : EntityEventArgs
{
    public NetEntity User;

    public bool Handled;
}
