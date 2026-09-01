using Content.Shared._Paradise.SiliconComponents;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Paradise.SiliconComponents.UI;

[UsedImplicitly]
public sealed class SiliconBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private SiliconComponentsMenu? _menu;

    public SiliconBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SiliconComponentsMenu>();
        _menu.SetEntity(Owner);

        _menu.UpdateIntegrityBars();

        _menu.BrainButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Brain));
        };

        _menu.OpticsButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Optics));
        };

        _menu.ServoButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Servo));
        };

        _menu.SpineButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Spine));
        };

        _menu.DriveButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Drive));
        };

        _menu.EjectBatteryButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectBatteryBuiMessage());
        };

        _menu.RemoveModuleButtonPressed += module =>
        {
            SendPredictedMessage(new SiliconRemoveModuleBuiMessage(EntMan.GetNetEntity(module)));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not SiliconBoundUiState msg)
            return;

        _menu?.UpdateIntegrityBars();
    }

    public override void Update()
    {
        _menu?.UpdateBatteryButton();
        _menu?.UpdateButtons();
        _menu?.UpdateModulePanel();
    }
}
