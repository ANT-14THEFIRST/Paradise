using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Paradise.Mech;

[Serializable, NetSerializable]
public enum MechUiKey : byte
{
    Key
}


[Serializable, NetSerializable]
public sealed class MechPartRemoveMessage : BoundUserInterfaceMessage
{
    public PartSlot Part;

    public MechPartRemoveMessage(PartSlot part)
    {
        Part = part;
    }
}

[Serializable, NetSerializable]
public sealed class AltMechEquipmentRemoveMessage : BoundUserInterfaceMessage
{
    public NetEntity Equipment;

    public AltMechEquipmentRemoveMessage(NetEntity equipment)
    {
        Equipment = equipment;
    }
}

[Serializable, NetSerializable]
public sealed class MechMaintenanceToggleMessage : BoundUserInterfaceMessage
{
    public bool Toggled;

    public MechMaintenanceToggleMessage(bool toggled)
    {
        Toggled = toggled;
    }
}

[Serializable, NetSerializable]
public sealed class MechSealMessage : BoundUserInterfaceMessage
{
    public bool Toggled;

    public MechSealMessage(bool toggled)
    {
        Toggled = toggled;
    }
}

[Serializable, NetSerializable]
public sealed class MechBoltMessage : BoundUserInterfaceMessage
{
    public bool Toggled;

    public MechBoltMessage(bool toggled)
    {
        Toggled = toggled;
    }
}

[Serializable, NetSerializable]
public sealed class MechDetachTankMessage : BoundUserInterfaceMessage
{
    public bool Toggled;

    public MechDetachTankMessage(bool toggled)
    {
        Toggled = toggled;
    }
}

[Serializable, NetSerializable]
public sealed class AltMechBoundUiState : BoundUserInterfaceState
{
    public Dictionary<NetEntity, BoundUserInterfaceState> EquipmentStates = new();

    public FixedPoint2 TankPressure;

    public FixedPoint2 TankTemperature;
}

public sealed class MechEquipmentUiStateReadyEvent : EntityEventArgs
{
    public Dictionary<NetEntity, BoundUserInterfaceState> States = new();
}

public sealed class MechEquipmentUiMessageRelayEvent : EntityEventArgs
{
    public MechEquipmentUiMessage Message;

    public MechEquipmentUiMessageRelayEvent(MechEquipmentUiMessage message)
    {
        Message = message;
    }
}

[Serializable, NetSerializable]
public sealed class MechEquipmentRemoveMessage : BoundUserInterfaceMessage
{
    public NetEntity Equipment;

    public MechEquipmentRemoveMessage(NetEntity equipment)
    {
        Equipment = equipment;
    }
}

[Serializable, NetSerializable]
public abstract class MechEquipmentUiMessage : BoundUserInterfaceMessage
{
    public NetEntity Equipment;
}

[Serializable, NetSerializable]
public sealed class MechGrabberEjectMessage : MechEquipmentUiMessage
{
    public NetEntity Item;

    public MechGrabberEjectMessage(NetEntity equipment, NetEntity uid)
    {
        Equipment = equipment;
        Item = uid;
    }
}

[Serializable, NetSerializable]
public sealed class MechSoundboardPlayMessage : MechEquipmentUiMessage
{
    public int Sound;

    public MechSoundboardPlayMessage(NetEntity equipment, int sound)
    {
        Equipment = equipment;
        Sound = sound;
    }
}

[Serializable, NetSerializable]
public sealed class MechBoundUiState : BoundUserInterfaceState
{
    public Dictionary<NetEntity, BoundUserInterfaceState> EquipmentStates = new();
}

[Serializable, NetSerializable]
public sealed class MechGrabberUiState : BoundUserInterfaceState
{
    public List<NetEntity> Contents = new();
    public int MaxContents;
}

[Serializable, NetSerializable]
public sealed class MechSoundboardUiState : BoundUserInterfaceState
{
    public List<string> Sounds = new();
}

[Serializable, NetSerializable]
public enum MechVisualLayers : byte
{
    Base
}
