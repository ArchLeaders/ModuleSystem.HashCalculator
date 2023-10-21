namespace ModuleSystem.HashCalculator.Helpers;

public class ModuleSystemPath
{
    public string RstbPath { get; }
    public string ZsDicPath { get; }

    public ModuleSystemPath(string path, string version)
    {
        RstbPath = Path.Combine(path, "System", "Resource", $"ResourceSizeTable.Product.{version.Replace(".", string.Empty)}.rsizetable.zs");
        ZsDicPath = Path.Combine(path, "Pack", "ZsDic.pack.zs");
    }

    public static bool IsValid(string path, string version, out ModuleSystemPath msPath)
    {
        msPath = new(path, version);
        return File.Exists(msPath.RstbPath);
    }
}
