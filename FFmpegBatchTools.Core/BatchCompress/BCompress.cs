using System.Text;
using FFmpegBatchTools.Core.Utils;

namespace FFmpegBatchTools.Core.BatchCompress;

public class BCompress : BatchBase<Config>
{
    public override string Suffix { get; set; } = "_nvenc";

    public override bool Run(string inPath, string outputDir, out string outPath)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(inPath);
        var extension = Config.UseCustomFileExtension ? Config.CustomFileExtension : Path.GetExtension(inPath);
        var output = Path.Combine(outputDir, nameWithoutExtension + Suffix + extension);

        var arguments = new StringBuilder($"-i {inPath.Quote()} -o {output.Quote()}");
        arguments.Append(Config.Use265 ? " -c hevc --tier high" : " -c h264 --profile high");
        arguments.Append(
            Config.UseQVBR
                ? $" --qvbr {Config.QVBR} --max-bitrate {Config.VBRMaxBitrate} --multipass 2pass-full"
                : $" --cqp {Config.CQP}"
        );

        arguments.Append(" --lookahead 32 -u P7 --audio-copy --sub-copy --chapter-copy --data-copy --attachment-copy");
        arguments.Append($" --log-level {Config.LogLevel} --log-opt addtime=on {Config.ExtraArguments}");

        var code = ProcessUtils.StartProcess("NVEncC64.exe", arguments.ToString());

        if (code != 0)
        {
            outPath = output;
            return false;
        }

        var inFile = new FileInfo(inPath);
        var outFile = new FileInfo(output);

        Console.WriteLine($"Old file {Helpers.GetBytesReadable(inFile.Length)}," +
                          $" New file {Helpers.GetBytesReadable(outFile.Length)}");

        if (outFile.Length > inFile.Length)
        {
            Console.WriteLine($"New file is larger, deleting: {output}");
            outFile.Delete();
            outPath = inPath; // report the input as the resulted output
            return true;
        }

        outPath = output;
        return true;
    }
}