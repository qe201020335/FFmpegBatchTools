using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FFmpegBatchTools.Shared
{
    public static class Utils
    {
        public static int StartProcess(string exe, string arguments)
        {
            Console.WriteLine($"Launching {exe} with arguments: {arguments}");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
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
            
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"{process.StartInfo.FileName} Exited with error!");
                switch (MessageBox.Show($"{process.StartInfo.FileName} Exited with error! Continue?", "Error!", MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        Console.WriteLine("Continue.");
                        return 1;
                    case DialogResult.No:
                        Console.WriteLine("Abort.");
                        return -1;
                    default:
                        return 0;
                }
            }

            return 0;
        }

        public static string Quote(this string s)
        {
            return "\"" + s + "\"";
        }
        
        public static string FindFFmpeg()
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            var paths = path.Split(';');
            foreach (var p in paths)
            {
                if (p.Contains("ffmpeg"))
                {
                    return p;
                }
            }

            return null;
        }
        
        public static string? FindNVEncC(string baseFolder)
        {
            var path = Path.Combine(baseFolder, "NVEncC", "NVEncC64.exe");
            return File.Exists(path) ? path : null;
        }
    }
}