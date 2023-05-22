using Newtonsoft.Json;

namespace FFmpegBatchTools.Core.BatchMerge;

public class Config
{
    [JsonProperty("SetTrackLanguage")] public bool SetTrackLanguage = false;

    [JsonProperty("TrackLanguageMap")] public Dictionary<string, string> TrackLanguageMap = new();
}