using Robust.Shared.Serialization;

namespace Content.Shared._Paradise.SiliconComponents;

[Serializable, NetSerializable]
public enum SiliconUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class SiliconEjectPartBuiMessage : BoundUserInterfaceMessage
{
    public PartType DesiredPart;

    public SiliconEjectPartBuiMessage(PartType desiredPart)
    {
        DesiredPart = desiredPart;
    }
}

[Serializable, NetSerializable]
public sealed class SiliconEjectBatteryBuiMessage : BoundUserInterfaceMessage;


[Serializable, NetSerializable]
public sealed class SiliconRemoveModuleBuiMessage(NetEntity module) : BoundUserInterfaceMessage
{
    public NetEntity Module = module;
}
