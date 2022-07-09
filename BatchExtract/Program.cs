using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFMpegCore;

namespace BatchExtract
{
    internal class Program
    {
        private static string prgname;
        private static void Extract(string path)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var parent = Path.GetDirectoryName(path);
            var extension = Path.GetExtension(path);
            var newpath = parent + "\\" + nameWithoutExtension + "_extracted" + extension;

            try
            {
                // var process = new Process
                // {
                //     StartInfo = new ProcessStartInfo
                //     {
                //         FileName = "ffmpeg.exe",
                //         Arguments = $"-v verbose -i \"{path}\" -map 0:v -map 0:a:3 -c copy -y \"{newpath}\"",
                //         UseShellExecute = false, 
                //         CreateNoWindow = true,
                //         RedirectStandardOutput = true,
                //         RedirectStandardInput = true,
                //         RedirectStandardError = true,
                //     }
                // };
                // process.Start();
                // process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                // process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data); 
                // process.BeginOutputReadLine();
                // process.BeginErrorReadLine();
                // process.WaitForExit(-1);
                FFMpegArguments.FromFileInput(path)
                    .OutputToFile(newpath, true, options => options.SelectStream(0).SelectStream(4).CopyChannel())
                    .ProcessSynchronously();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Main(string[] args)
        {
            AssemblyName ver = typeof(Program).Assembly.GetName();
            prgname = ver.Name;
            
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
                    tasks.Add(Task.Factory.StartNew(() => Extract(fullpath)));
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