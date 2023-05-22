using Newtonsoft.Json;

namespace FFmpegBatchTools.Core;

public class Configuration
{
    [JsonProperty("SelectOutputDir")] public bool SelectOutputDir = false;

    [JsonProperty("LastOutputDir")] public string LastOutputDir = "";

    [JsonProperty("DeleteOriginal")] public bool DeleteOriginal = true;

    [JsonProperty("CopyModifiedTime")] public bool CopyModifiedTime = true;

    [JsonProperty("CopyFileName")] public bool CopyFileName = true;

    [JsonProperty("BatchCompress")] public BatchCompress.Config BatchCompress = new();
    
    [JsonProperty("BatchMerge")] public BatchMerge.Config BatchMerge = new();
}