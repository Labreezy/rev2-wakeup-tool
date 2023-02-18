using System.Text.Json.Serialization;

namespace GGXrdReversalTool.Library.Configuration;

public class ReversalToolConfigObject
{
    [JsonPropertyName("Logging")]
    public Logging Logging { get; set; }

    [JsonPropertyName("GGProcessName")]
    public string GGProcessName { get; set; }

    [JsonPropertyName("P2IdOffset")]
    public string P2IdOffset { get; set; }

    [JsonPropertyName("RecordingSlotPtr")]
    public string RecordingSlotPtr { get; set; }

    [JsonPropertyName("P1AnimStringPtr")]
    public string P1AnimStringPtr { get; set; }

    [JsonPropertyName("P2AnimStringPtr")]
    public string P2AnimStringPtr { get; set; }

    [JsonPropertyName("FrameCountPtr")]
    public string FrameCountPtr { get; set; }

    [JsonPropertyName("ScriptOffset")]
    public string ScriptOffset { get; set; }

    [JsonPropertyName("P1ComboCountPtr")]
    public string P1ComboCountPtr { get; set; }

    [JsonPropertyName("P2ComboCountPtr")]
    public string P2ComboCountPtr { get; set; }

    [JsonPropertyName("UpdateLink")]
    public string UpdateLink { get; set; }

    [JsonPropertyName("DummyIdPtr")]
    public string DummyIdPtr { get; set; }

    [JsonPropertyName("P1ReplayKeyPtr")]
    public string P1ReplayKeyPtr { get; set; }

    [JsonPropertyName("P2ReplayKeyPtr")]
    public string P2ReplayKeyPtr { get; set; }

    [JsonPropertyName("P2BlockStunPtr")]
    public string P2BlockStunPtr { get; set; }

    [JsonPropertyName("ReplayTriggerType")]
    public string ReplayTriggerType { get; set; }

    [JsonPropertyName("CurrentVersion")]
    public Version CurrentVersion { get; set; }

    [JsonPropertyName("AutoUpdate")]
    public bool AutoUpdate { get; set; }
}

public class Logging
{
    [JsonPropertyName("LogLevel")]
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    [JsonPropertyName("Default")]
    public string Default { get; set; }

    [JsonPropertyName("System")]
    public string System { get; set; }

    [JsonPropertyName("Microsoft")]
    public string Microsoft { get; set; }
}