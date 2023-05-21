namespace FFmpegBatchTools.Core;

public abstract class BatchBase<T> where T : new()
{
    public abstract string Suffix { get; set; }

    public T Config { get; set; } = new T();

    public abstract bool Run(string inPath, string outputDir, out string outPath);
}