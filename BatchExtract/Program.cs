using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFMpegCore;
using Newtonsoft.Json;

namespace BatchExtract
{
    internal class Program
    {
        private const string SUFFIX = "_extracted";
        private static string prgname;
        private static string ConfName;
        private static string ConfPath;
        private static Configuration Configuration => Configuration.Instance;

        private static async Task Extract(string path)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var parent = Path.GetDirectoryName(path);
            var extension = Path.GetExtension(path);
            var newpath = parent + "\\" + nameWithoutExtension + SUFFIX + extension;

            try
            {

                await FFMpegArguments.FromFileInput(path)
                    .OutputToFile(newpath, true,
                        options => Configuration.ExtractStreams.Aggregate(options, (op, num) => op.SelectStream(num))
                            .CopyChannel()).ProcessAsynchronously();
                
                var oldFile = new FileInfo(path);
                var newFile = new FileInfo(newpath);
                
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
            
            int len = args.Length;
            if (len == 0)
            {
                MessageBox.Show("Drag video files on this tool.", prgname);
                return;
            }

            List<Task> tasks = new List<Task>();
            
            foreach (string arg in args)
            {
                try
                {
                    if (!File.Exists(arg))
                    {
                        Console.WriteLine(arg + " DOES NOT EXIST");
                        continue;  // not exist
                    }
                    var fullpath = Path.GetFullPath(arg);
                    tasks.Add(Extract(fullpath));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Task.WaitAll(tasks.ToArray());

            Console.Write("\a");
            MessageBox.Show("Done.", prgname);
        }
    }
}