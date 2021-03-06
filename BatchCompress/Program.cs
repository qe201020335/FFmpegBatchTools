using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private static Configuration Configuration => Configuration.Instance;
        private static void Run(string path)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var parent = Path.GetDirectoryName(path);
            var extension = Path.GetExtension(path);

            var newpath = parent + "\\" + nameWithoutExtension + SUFFIX + extension;
            
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "NVEncC64.exe",
                        Arguments = $"-i \"{path}\" --cqp {Configuration.CQP} -u P7 --audio-copy -o \"{newpath}\" --log-level {Configuration.LogLevel} --log-opt addtime=on",
                        UseShellExecute = false, 
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                    }
                };
                process.Start();
                process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data); 
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(-1);

                var oldFile = new FileInfo(path);
                var newFile = new FileInfo(newpath);

                Console.WriteLine($"Old file {Utils.GetBytesReadable(oldFile.Length)}," +
                                  $" New file {Utils.GetBytesReadable(newFile.Length)}");
                
                if (newFile.Length > oldFile.Length)
                {
                    Console.WriteLine($"new file is larger, deleting: {newpath}");
                    newFile.Delete();
                }
                else
                {
                    if (Configuration.CopyModifiedTime)
                    {
                        newFile.LastWriteTime = oldFile.LastWriteTime;
                    }
                    if (Configuration.DeleteOriginal)
                    {
                        oldFile.Delete();
                        Console.WriteLine($"old file deleted: {path}");
                        if (Configuration.CopyFileName)
                        {
                            Console.WriteLine("Rename to old file name");
                            newFile.MoveTo(path);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        private static void DealWithConfig()
        {
            if (File.Exists(ConfPath))
            {
                using (var file = File.OpenText(ConfPath))
                {
                    var serializer = new JsonSerializer();
                    Configuration.Instance = (Configuration) serializer.Deserialize(file, typeof(Configuration));
                }
            }
            else
            {
                Configuration.Instance = new Configuration();
            }
            //TODO Show a config menu
            using (var file = File.CreateText(ConfPath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Configuration.Instance);
            }
        }
        
        public static void Main(string[] args)
        {
            var ass = Assembly.GetExecutingAssembly();
            prgname = ass.GetName().Name;
            ConfName = prgname + ".json";
            ConfPath = Path.Combine(Path.GetDirectoryName(ass.Location), ConfName);
            
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
                        continue;  // not exist
                    }
                    var fullpath = Path.GetFullPath(arg);
                    Run(fullpath);
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