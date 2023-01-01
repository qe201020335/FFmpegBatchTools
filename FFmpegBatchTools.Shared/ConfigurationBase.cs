using Newtonsoft.Json;

namespace FFmpegBatchTools.Shared
{
    public abstract class ConfigurationBase
    {
        [JsonProperty("DeleteOriginal")] public bool DeleteOriginal = false;

        [JsonProperty("CopyModifiedTime")] public bool CopyModifiedTime = false;

        [JsonProperty("CopyFileName")] public bool CopyFileName = true;

        [JsonProperty("SelectOutputDir")]
        public bool SelectOutputDir = false;

        [JsonProperty("UseCustomFileExtension")]
        public bool UseCustomFileExtension = false;

        [JsonProperty("CustomFileExtension")]
        public string CustomFileExtension = ".mkv";

        [JsonProperty("LastOutputDir")]
        public string LastOutputDir = "";

        [JsonProperty("ExtraArguments")]
        public string ExtraArguments = "";
    }
}