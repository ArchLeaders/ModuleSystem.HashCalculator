namespace ModuleSystem.HashCalculator.Helpers;

public class TotkPath
{
    public string RstbPath { get; }
    public string ZsDicPath { get; }

    public TotkPath(string path, string version)
    {
        RstbPath = Path.Combine(path, "System", "Resource", $"ResourceSizeTable.Product.{version.Replace(".", string.Empty)}.rsizetable.zs");
        ZsDicPath = Path.Combine(path, "Pack", "ZsDic.pack.zs");
    }

    public static bool IsValid(string path, string version, out TotkPath totkPath)
    {
        totkPath = new(path, version);
        return File.Exists(totkPath.RstbPath) && File.Exists(totkPath.ZsDicPath);
    }
}
