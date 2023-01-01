using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FFmpegBatchTools.Shared
{
    public abstract class ProgramBase<TConfig> where TConfig : ConfigurationBase, new()
    {
        protected readonly string PrgName;
        protected readonly string ConfName;
        protected readonly string ConfPath;
        protected readonly string ProgramDir;

        protected TConfig Configuration => _config!;
        
        private TConfig? _config = null;

        protected bool _useSelectedDir = false;
        private string? _selectedDir;
        private FolderBrowserDialog _outputDirDialog = new FolderBrowserDialog();

        protected ProgramBase(string prgName, string confName, string confPath, string programDir, Func<TConfig>? newConfig = null)
        {
            PrgName = prgName;
            ConfName = confName;
            ConfPath = confPath;
            ProgramDir = programDir;
            
            DealWithConfig(newConfig);
        }
        
        private void DealWithConfig(Func<TConfig>? newConfig)
        {
            if (File.Exists(ConfPath))
            {
                using var file = File.OpenText(ConfPath);
                
                var serializer = new JsonSerializer();
                _config = serializer.Deserialize(file, typeof(TConfig)) as TConfig;
            }

            _config ??= newConfig?.Invoke() ?? new TConfig();

            if (Configuration.SelectOutputDir)
            {
                _outputDirDialog.SelectedPath = Directory.Exists(Configuration.LastOutputDir) ? Configuration.LastOutputDir : ProgramDir;
                if (_outputDirDialog.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine(_outputDirDialog.SelectedPath);
                    _useSelectedDir = true;
                    _selectedDir = _outputDirDialog.SelectedPath;
                    Configuration.LastOutputDir = _outputDirDialog.SelectedPath;
                }
            }

            //TODO Show a config menu
            using (var file = File.CreateText(ConfPath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Configuration);
            }
        }
    }
}