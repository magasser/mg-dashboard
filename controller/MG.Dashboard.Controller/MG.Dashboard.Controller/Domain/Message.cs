namespace MG.Dashboard.Controller.Domain;

public sealed record Message
{
    public const char MessageSeparator = ';';
    public const char TypeSeparator = '.';
    public const char ParameterSeparator = ',';

    public static readonly string[] ValidTypes = { Types.Command, Types.State, Types.Mode, Types.Acknowledge };

    public static Message Invalid = new(Types.Invalid);

    private Message(string type, params object[] parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(type));
        }

        Type = type;
        Parameters = parameters;
    }

    public string Type { get; }

    public object[] Parameters { get; }

    public override string ToString() =>
        $"{Type}{TypeSeparator}{string.Join(ParameterSeparator, Parameters)}{MessageSeparator}";

    public static Message FromString(string message)
    {
        var parts = message.Trim(MessageSeparator).Split(TypeSeparator);

        if (parts.Length is not 1 and not 2)
        {
            return Invalid;
        }

        var type = parts[0];

        if (string.IsNullOrWhiteSpace(type) || !ValidTypes.Contains(type))
        {
            return Invalid;
        }

        var parameters = parts.Length is 2 ? parts[1].Split(ParameterSeparator) : Array.Empty<string>();

        return new Message(type, parameters);
    }

    public static Message FromTypeAndParameters(string type, params object[] parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (string.IsNullOrWhiteSpace(type) || !ValidTypes.Contains(type))
        {
            return Invalid;
        }

        return new Message(type, parameters);
    }

    public static class Types
    {
        public const string Invalid = "inv";
        public const string Command = "cmd";
        public const string State = "ste";
        public const string Mode = "mod";
        public const string Connection = "con";
        public const string Acknowledge = "ack";
    }

    public static class Commands
    {
        public const string TurnLeft = "tl";
        public const string TurnRight = "tr";
        public const string Forward = "fw";
        public const string Backward = "bw";
    }

    public static class States
    {
        public const string Running = "run";
        public const string Error = "err";
    }

    public static class Modes
    {
        public const string Automatic = "auto";
        public const string Controlled = "cont";
    }
}
