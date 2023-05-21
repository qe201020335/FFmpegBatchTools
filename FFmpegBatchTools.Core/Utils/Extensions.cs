namespace FFmpegBatchTools.Core.Utils;

public static class Extensions
{
    public static string Quote(this string s)
    {
        return "\"" + s + "\"";
    }
}