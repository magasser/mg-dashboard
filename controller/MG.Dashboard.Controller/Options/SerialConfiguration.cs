namespace MG.Dashboard.Controller.Options;

public sealed record SerialConfiguration
{
    public const string Key = "SerialConfiguration";

    public int BaudRate { get; init; } = 9600;

    public int ReadWriteTimeout { get; init; } = 100;

    public int ConnectionTimeout { get; init; } = 10000;

    public string MessageSeparator { get; init; } = ";";
}
