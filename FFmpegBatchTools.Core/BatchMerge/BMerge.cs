using System.Text;
using FFmpegBatchTools.Core.Utils;

namespace FFmpegBatchTools.Core.BatchMerge;

public class BMerge : BatchBase<Config>
{
    public override string Suffix { get; set; } = "_merged";

    public override bool Run(string inPath, string outputDir, out string outPath)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(inPath);
        var inParent = Path.GetDirectoryName(inPath)!;
        outPath = Path.Combine(outputDir, nameWithoutExtension + Suffix + ".mkv");

        var inputFiles = Directory
            .GetFiles(inParent)
            .Where(fileName => Path
                .GetFileNameWithoutExtension(fileName)
                .ToLower()
                .StartsWith(nameWithoutExtension.ToLower()))
            .Aggregate(new StringBuilder(), (s, fileName) => s.Append(' ').Append(fileName.Quote()));


        var result1 = ProcessUtils.StartProcess("mkvmerge.exe", $"{inputFiles} -o {outPath.Quote()}");
        if (result1 != 0)
        {
            return false;
        }

        string? fonts = null;
        if (Directory.Exists(Path.Combine(inParent, "fonts")))
        {
            fonts = Directory
                .GetFiles(Path.Combine(inParent, "fonts"))
                .Aggregate(new StringBuilder(" "), (s, fileName) => s.Append($" --add-attachment {fileName.Quote()}"))
                .ToString();
        }

        string? tracks = null;
        if (Config.SetTrackLanguage)
        {
            tracks = Config.TrackLanguageMap
                .Aggregate(new StringBuilder(" "), (s, trackLang) => s.Append($" --edit track:{trackLang.Key} --set language={trackLang.Value}"))
                .ToString();
        }

        if (fonts == null && tracks == null) return true;  // nothing more to do
        
        var result2 = ProcessUtils.StartProcess("mkvpropedit.exe", $"{outPath.Quote()}{fonts ?? ""}{tracks ?? ""}");
        return result2 == 0;
    }
}