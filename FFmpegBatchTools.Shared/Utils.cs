using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FFmpegBatchTools.Shared
{
    public class Utils
    {
        public static int StartProcess(string exe, string arguments)
        {
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
    }
}