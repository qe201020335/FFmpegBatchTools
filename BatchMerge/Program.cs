using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FFmpegBatchTools.Shared;

namespace BatchMerge
{
    internal class Program : ProgramBase<Configuration>
    {

        private int? _subTrack;

        protected override bool Run(string inPath)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(inPath);
            var inParent = Path.GetDirectoryName(inPath)!;
            var extension = ".mkv";
            var outParent = UseSelectedDir ? SelectedDir ?? inParent : inParent;
            var outPath = Path.Combine(outParent,  nameWithoutExtension + "_merged" + extension);

            try
            {
                var inputFiles = Directory
                    .GetFiles(inParent)
                    .Where(fileName => Path
                        .GetFileNameWithoutExtension(fileName)
                        .ToLower()
                        .StartsWith(nameWithoutExtension.ToLower()))
                    .Aggregate(new StringBuilder(), (s, fileName) =>s.Append(" ").Append(fileName.Quote()));
                
                
                var result1 = Utils.StartProcess("mkvmerge.exe", $"{inputFiles} -o {outPath.Quote()}");
                if (result1 > 1) return true;
                if (result1 < 0) return false;

                string fonts = "";
                if (Directory.Exists(Path.Combine(inParent, "fonts")))
                {
                    fonts = Directory
                        .GetFiles(Path.Combine(inParent, "fonts"))
                        .Aggregate(new StringBuilder(" "), (s, fileName) => s.Append($" --add-attachment {fileName.Quote()}"))
                        .ToString();
                }

                var subArg = _subTrack == null ? "" : $" --edit track:s{_subTrack} --set language=zh";
                var result2 = Utils.StartProcess("mkvpropedit.exe", $"{outPath.Quote()}{fonts}{subArg}");
                if (result2 > 1) return true;
                if (result2 < 0) return false;

                var inFile = new FileInfo(inPath);
                var outFile = new FileInfo(outPath);
                

                if (Configuration.CopyModifiedTime)
                {
                    outFile.LastWriteTime = inFile.LastWriteTime;
                }

                // if (Configuration.DeleteOriginal)
                // {
                //     inFile.Delete();
                //     Console.WriteLine($"old file deleted: {inPath}");
                // }

                if (Configuration.CopyFileName)
                {
                    var newName = Path.Combine(outParent, nameWithoutExtension + extension);
                    if (newName != inPath)
                    {
                        Console.WriteLine("Rename to old file name");
                        outFile.MoveTo(newName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var program = new Program();
            
            Console.WriteLine("Subtitle Track? (Start from 1)");
            if (int.TryParse(Console.ReadLine(), out var i))
            {
                program._subTrack = i;
            }

            if (!program.BatchRun(args))
            {
                Console.WriteLine("\a");
                Console.WriteLine("There are errors\nPress Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}