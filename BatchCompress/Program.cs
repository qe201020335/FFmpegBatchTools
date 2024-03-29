﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFmpegBatchTools.Shared;
using FFMpegCore;
using FFMpegCore.Enums;
using Newtonsoft.Json;

namespace BatchCompress
{
    internal class Program
    {
        private const string SUFFIX = "_nvenc";
        private static string prgname;
        private static string ConfName;
        private static string ConfPath;
        private static string ProgramDir;
        private static Configuration Configuration => Configuration.Instance;

        private static bool _useSelectedDir = false;
        private static string _selectedDir;
        private static FolderBrowserDialog _outputDirDialog = new FolderBrowserDialog();

        private static bool Run(string inPath)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(inPath);
            var inParent = Path.GetDirectoryName(inPath);
            var extension = Configuration.UseCustomFileExtension ? Configuration.CustomFileExtension : Path.GetExtension(inPath);
            var outParent = _useSelectedDir ? _selectedDir : inParent;
            var outPath = outParent + "\\" + nameWithoutExtension + SUFFIX + extension;

            try
            {
                var arguments = new StringBuilder($"-i {inPath.Quote()} -o {outPath.Quote()}");
                if (Configuration.Use265)
                {
                    arguments.Append(" -c hevc --tier high");
                }
                else
                {
                    arguments.Append(" -c h264 --profile high");
                }

                arguments.Append(
                    Configuration.UseQVBR
                        ? $" --qvbr {Configuration.QVBR} --max-bitrate {Configuration.VBRMaxBitrate} --multipass 2pass-full"
                        : $" --cqp {Configuration.CQP}"
                );


                arguments.Append(" --lookahead 32 -u P7 --audio-copy --sub-copy --chapter-copy --data-copy --attachment-copy");
                arguments.Append($" --log-level {Configuration.LogLevel} --log-opt addtime=on {Configuration.ExtraArguments}");
                var code = FFmpegBatchTools.Shared.Utils.StartProcess("NVEncC64.exe", arguments.ToString());
                
                switch (code)
                {
                    case 1:
                        return true;
                    case -1:
                        return false;
                }

                var inFile = new FileInfo(inPath);
                var outFile = new FileInfo(outPath);

                Console.WriteLine($"Old file {Utils.GetBytesReadable(inFile.Length)}," +
                                  $" New file {Utils.GetBytesReadable(outFile.Length)}");

                if (outFile.Length > inFile.Length)
                {
                    Console.WriteLine($"New file is larger, deleting: {outPath}");
                    outFile.Delete();
                    return true;
                }
                
                if (Configuration.CopyModifiedTime)
                {
                    outFile.LastWriteTime = inFile.LastWriteTime;
                }

                if (Configuration.DeleteOriginal)
                {
                    inFile.Delete();
                    Console.WriteLine($"old file deleted: {inPath}");
                }

                if (Configuration.CopyFileName )
                {
                    var newName = Path.Combine(outParent, nameWithoutExtension + extension);
                    if (Configuration.DeleteOriginal || newName != inPath)
                    {
                        Console.WriteLine("Rename to old file name");
                        outFile.MoveTo(newName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }

        private static void DealWithConfig()
        {
            if (File.Exists(ConfPath))
            {
                using (var file = File.OpenText(ConfPath))
                {
                    var serializer = new JsonSerializer();
                    Configuration.Instance = (Configuration)serializer.Deserialize(file, typeof(Configuration));
                }
            }
            else
            {
                Configuration.Instance = new Configuration();
            }
            
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
                serializer.Serialize(file, Configuration.Instance);
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var ass = Assembly.GetExecutingAssembly();
            prgname = ass.GetName().Name;
            ConfName = prgname + ".json";
            ProgramDir = Path.GetDirectoryName(ass.Location) ?? "";
            ConfPath = Path.Combine(ProgramDir, ConfName);

            DealWithConfig();

            var len = args.Length;
            if (len == 0)
            {
                MessageBox.Show("Drag video files on this tool.", prgname);
                return;
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
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.Write("\a");
            MessageBox.Show("Done.", prgname);
        }
    }
}