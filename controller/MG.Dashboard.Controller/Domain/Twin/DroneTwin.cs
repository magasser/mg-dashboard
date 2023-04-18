namespace MG.Dashboard.Controller.Domain.Twin;

public sealed record DroneTwin : DeviceTwin
{
    public DroneTwin(string id, string name)
        : base(id, name, DeviceType.Car) { }

    /// <inheritdoc />
    public override DeviceTwin Update(Message message) => throw new NotImplementedException();
}
