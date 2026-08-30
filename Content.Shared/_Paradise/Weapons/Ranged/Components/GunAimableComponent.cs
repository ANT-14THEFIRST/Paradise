using Robust.Shared.GameStates;

namespace Content.Shared._Paradise.Weapons.Components;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]

public sealed partial class GunAimableComponent : Component//This component signalises that the gun can be used for aimed shooting
{
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public bool IsAimed = false; //True when aimed. Aiming state applies following parameters

    [ViewVariables(VVAccess.ReadWrite), DataField("minAngle")]
    public Angle MinAngle = Angle.FromDegrees(0); 

    /// <summary>
    /// Angle bonus applied upon being aimed.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("maxAngle")]
    public Angle MaxAngle = Angle.FromDegrees(0);

    /// <summary>
    /// Recoil bonuses applied upon being aimed.
    /// Higher angle decay bonus, quicker recovery.
    /// Lower angle increase bonus (negative numbers), slower buildup.
    /// </summary>
    [DataField]
    public Angle AngleDecay = Angle.FromDegrees(0);

    /// <summary>
    /// Recoil bonuses applied upon being aimed.
    /// Higher angle decay bonus, quicker recovery.
    /// Lower angle increase bonus (negative numbers), slower buildup.
    /// </summary>
    [DataField]
    public Angle AngleIncrease = Angle.FromDegrees(0);

    [DataField]
    public float? AimedSprintSpeedModifier = 0.5f; //User's speed is multiplied by this value

    [DataField]
    public float? AimedWalkingSpeedModifier = 1f;
}
