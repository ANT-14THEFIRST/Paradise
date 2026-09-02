using Content.Shared.FixedPoint;
using Content.Shared.Stacks;
using Content.Shared.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Paradise.ComplexRepairable;

/// <summary>
/// Use this component to mark a device as repairable.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]

public sealed partial class ComplexRepairableComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 DamageValue = -10;

    /// <summary>
    /// Cost of fuel used to repair this device.
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 FuelCost = 5;

    [AutoNetworkedField]
    public FixedPoint2 DamageSinceLastThresholdUpdate = 0;

    /// <summary>
    /// Material used to fix the owner of the component 
    /// </summary>
    [DataField]
    public ProtoId<StackPrototype> Material;

    /// <summary>
    /// How much of given material the user has to insert in order to repair
    /// </summary>
    [AutoNetworkedField]
    public int LeftToInsert;

    /// <summary>
    /// When total damage reaches this value the user will have to use one piece of specified material. This multiplies with damage
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 MaterialRepairThreshold;

    /// <summary>
    /// Tool quality necessary to repair this device.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<ToolQualityPrototype> QualityNeeded = "Welding";

    /// <summary>
    /// Time needed to repair the entity
    /// </summary>
    [DataField, AutoNetworkedField]
    public float RepairTime = 1f;

    /// <summary>
    /// A multiplier that will be applied to the above if an entity is repairing themselves.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float SelfRepairPenalty = 1f;

    /// <summary>
    /// Whether an entity is allowed to repair itself.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool AllowSelfRepair = true;

    /// <summary>
    /// If true and after the repair there still damage, a new doafter starts automatically
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool AutoDoAfter = true;
}
