using System;
using System.IO;
using System.Reflection;
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

        protected bool UseSelectedDir = false;
        protected string? SelectedDir;
        
        private FolderBrowserDialog _outputDirDialog = new FolderBrowserDialog();

        protected ProgramBase(string prgName, string confName, string confPath, string programDir, Func<TConfig>? newConfig = null)
        {
            PrgName = prgName;
            ConfName = confName;
            ConfPath = confPath;
            ProgramDir = programDir;
            
            DealWithConfig(newConfig);
        }

        protected ProgramBase()
        {
            var ass = Assembly.GetCallingAssembly();
            var prgName = ass.GetName().Name;
            var confName = prgName + ".json";
            var programDir = Path.GetDirectoryName(ass.Location) ?? "";
            var confPath = Path.Combine(programDir, confName); 
            
            PrgName = prgName;
            ConfName = confName;
            ConfPath = confPath;
            ProgramDir = programDir;
            
            DealWithConfig(null);
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
                    UseSelectedDir = true;
                    SelectedDir = _outputDirDialog.SelectedPath;
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

        protected abstract bool Run(string inPath);

        protected bool BatchRun(string[] args)
        {
            var len = args.Length;
            if (len == 0)
            {
                MessageBox.Show("Drag video files on this tool.", PrgName);
                return false;
            }

            foreach (var arg in args)
            {
                try
                {
                    if (!File.Exists(arg))
                    {
                        Console.WriteLine(arg + " DOES NOT EXIST");
                        continue; // not exist
                    }

                    var fullpath = Path.GetFullPath(arg);
                    if (!Run(fullpath))
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }

            Console.Write("\a");
            MessageBox.Show("Done.", PrgName);
            return true;
        }

        // protected void PostProcess(FileInfo inFile, FileInfo outFile)
        // {
        //     if (Configuration.CopyModifiedTime)
        //     {
        //         outFile.LastWriteTime = inFile.LastWriteTime;
        //     }
        //
        //     var inPath = inFile.FullName;
        //
        //     if (Configuration.DeleteOriginal)
        //     {
        //         inFile.Delete();
        //         Console.WriteLine($"old file deleted: {inPath}");
        //     }
        //
        //     if (Configuration.CopyFileName)
        //     {
        //         var newName = Path.Combine(outParent, nameWithoutExtension + extension);
        //         if (Configuration.DeleteOriginal || newName != inPath)
        //         {
        //             Console.WriteLine("Rename to old file name");
        //             outFile.MoveTo(newName);
        //         }
        //     }
        // }
    }
}