namespace ExcavationMethod.Abstractions.Telemetry;

public static class EventName
{
    public const string HostName = "Revit";
    public const string ToolPrefix = "Tool";
}

public static class AppAuthentication
{
    public static readonly string Success = $"{EventName.HostName}.{nameof(AppAuthentication)}.{nameof(Success)}";
    public static readonly string Failure = $"{EventName.HostName}.{nameof(AppAuthentication)}.{nameof(Failure)}";
}

public static class Addin
{
    public static readonly string Started = $"{EventName.HostName}.{nameof(Addin)}.{nameof(Started)}";
    public static readonly string Stopped = $"{EventName.HostName}.{nameof(Addin)}.{nameof(Stopped)}";
}
public static class Tool
{
    public static string Started(string toolName) => $"{EventName.HostName}.{EventName.ToolPrefix}.{toolName}.{nameof(Started)}";
    public static string Executed(string toolName) => $"{EventName.HostName}.{EventName.ToolPrefix}.{toolName}.{nameof(Executed)}";
    public static string Closed(string toolName) => $"{EventName.HostName}.{EventName.ToolPrefix}.{toolName}.{nameof(Closed)}";
    public static string Completed(string toolName) => $"{EventName.HostName}.{EventName.ToolPrefix}.{toolName}.{nameof(Completed)}";
}

public static class ThirdParty
{
    public static string Started(string toolName) => $"{EventName.HostName}.{nameof(ThirdParty)}.{toolName}.{nameof(Started)}";
}
