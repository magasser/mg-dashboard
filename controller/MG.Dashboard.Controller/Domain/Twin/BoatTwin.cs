namespace MG.Dashboard.Controller.Domain.Twin;

public sealed record BoatTwin : DeviceTwin
{
    public BoatTwin(string id, string name)
        : base(id, name, DeviceType.Car) { }

    /// <inheritdoc />
    public override BoatTwin Update(Message message) => throw new NotImplementedException();
}
