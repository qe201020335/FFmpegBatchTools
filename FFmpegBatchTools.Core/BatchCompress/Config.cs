using Newtonsoft.Json;

namespace FFmpegBatchTools.Core.BatchCompress;

public class Config
{
    [JsonIgnore] private static readonly HashSet<string> ValidLogLevels = new() { "error", "warn", "info", "debug", "trace" };

    [JsonProperty("CQP")] public float CQP = 18;

    [JsonProperty("QVBR")] public float QVBR = 23;

    [JsonProperty("VBRMaxBitrate")] public float VBRMaxBitrate = 240000;

    [JsonProperty("UseQVBR")] public bool UseQVBR = true;

    [JsonProperty("Use265")] public bool Use265 = true;

    [JsonProperty("LogLevel")]
    public string LogLevel
    {
        get => _logLevel;
        set
        {
            value = value.ToLower();
            _logLevel = ValidLogLevels.Contains(value) ? value : "info";
        }
    }

    [JsonIgnore] private string _logLevel = "info";

    public bool UseCustomFileExtension = true;

    public string CustomFileExtension = ".mp4";
    
    public string ExtraArguments = "";
}