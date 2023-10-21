namespace ModuleSystem.HashCalculator.Helpers;

public static class ModuleSystemPathExtension
{
    public static string Canonical(this string path)
    {
        return (!path.EndsWith(".ta.zs") && path.EndsWith(".zs") || path.EndsWith(".mc") ? path[..(^3)] : path).Replace('\\', '/');
    }
}
