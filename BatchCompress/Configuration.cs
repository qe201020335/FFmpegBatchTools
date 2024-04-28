using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BatchCompress
{
    public class Configuration
    {
        [JsonIgnore] public static Configuration Instance;

        [JsonIgnore] private static readonly HashSet<string> ValidLogLevels = new HashSet<string>
            { "error", "warn", "info", "debug", "trace" };

        [JsonProperty("CQP")] public float CQP = 18;
        
        [JsonProperty("QVBR")] public float QVBR = 18;
        
        [JsonProperty("VBRMaxBitrate")] public float VBRMaxBitrate = 240000;

        [JsonProperty("UseQVBR")] public bool UseQVBR = true;
        
        [JsonProperty("Use265")] public bool Use265 = true;
        

        [JsonProperty("DeleteOriginal")] public bool DeleteOriginal = false;

        [JsonProperty("CopyModifiedTime")] public bool CopyModifiedTime = true;

        [JsonProperty("CopyFileName")] public bool CopyFileName = true;

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

        [JsonProperty("SelectOutputDir")]
        public bool SelectOutputDir = true;

        public bool UseCustomFileExtension = false;

        public string CustomFileExtension = ".mov";

        public string LastOutputDir = "";

        public string ExtraArguments = "";
    }
}