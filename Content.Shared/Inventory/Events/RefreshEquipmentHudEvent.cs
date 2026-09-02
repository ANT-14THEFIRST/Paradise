using Content.Shared._Paradise.SiliconComponents;

namespace Content.Shared.Inventory.Events;

[ByRefEvent]
public record struct RefreshEquipmentHudEvent<T>(SlotFlags TargetSlots) : IInventoryRelayEvent, ISiliconPartRelayEvent //PARADISE EDIT - Silicon update
    where T : IComponent
{
    public SlotFlags TargetSlots { get; } = TargetSlots;
    public bool Active = false;
    public List<T> Components = new();

    PartType ISiliconPartRelayEvent.Parts => PartType.Optics; //PARADISE EDIT - Silicon update

    bool ISiliconPartRelayEvent.RelayToModules => true;//PARADISE EDIT - Silicon update
}
