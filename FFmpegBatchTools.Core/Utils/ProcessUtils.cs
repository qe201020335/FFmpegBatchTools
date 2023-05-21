using System.Diagnostics;

namespace FFmpegBatchTools.Core.Utils;

public class ProcessUtils
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
        
        return process.ExitCode;
    }
}