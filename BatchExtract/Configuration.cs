using Newtonsoft.Json;

namespace BatchExtract
{
    public class Configuration
    {
        [JsonIgnore] public static Configuration Instance;

        [JsonProperty("DeleteOriginal")] public bool DeleteOriginal = true;
        
        [JsonProperty("CopyModifiedTime")] public bool CopyModifiedTime = true;
        
        [JsonProperty("CopyFileName")] public bool CopyFileName = true;

        [JsonProperty("ExtractStreams")] public int[] ExtractStreams = { 0, 1 };

        // [JsonProperty("LogLevel")]
        // public string LogLevel
        // {
        //     get => _logLevel;
        //     set
        //     {
        //         value = value.ToLower();
        //         _logLevel = ValidLogLevels.Contains(value) ? value : "info";
        //     }
        // }
        //
        // [JsonIgnore] private string _logLevel = "info";
    }
}