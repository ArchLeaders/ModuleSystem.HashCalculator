using System.Reflection;
using System.Text.Json;
using ModuleSystem.HashCalculator;
using ModuleSystem.HashCalculator.Helpers;
using ModuleSystem.HashCalculator.Parsers;

Console.WriteLine($"""
    Module System Hash Calculator [Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "Undefined"}]
    (c) Arch Leaders. GNU Affero General Public License

    """);

bool isFloatingConsole = false;
string? path = null;

if (isFloatingConsole = args.Length <= 0) {
    await Console.Out.WriteLineAsync("Please enter the source directory: ");
}
else if (args[0] is "-h" or "--help") {
    await Console.Out.WriteLineAsync("""
        Usage:
          TotK-HashCalculator <path> [-o|--output OUTPUT] [-v|--version VERSION]
        """);

    return;
}
else {
    path = args[0];
}

Flags flags = Flags.Parse(args);

string version = flags.TryGet(out string? versionValue, "version", "v")
    ? versionValue! : "1.0.0";

ModuleSystemPath msPath = new(path ?? string.Empty, version);

while (!Directory.Exists(path ??= Console.ReadLine() ?? string.Empty) || !ModuleSystemPath.IsValid(path, version, out msPath)) {
    await Console.Out.WriteLineAsync($"Error: Invalid path '{path}'");
    await Console.Out.WriteAsync("Please enter the source directory: ");
    path = null;
}

ModuleSystemZstd zs = new(msPath.ZsDicPath);

await Console.Out.WriteLineAsync("Collecting file names...");
List<string> files = await FileIterator.Collect(path, zs);
await Console.Out.WriteLineAsync("""
    ;
    Operation successful.
    """);

await Console.Out.WriteLineAsync("Comparing collection against rstb...");
FilterUntrackedFiles(files, out Dictionary<uint, string> hashes);

await Console.Out.WriteLineAsync("Creating output...");
string output = flags.TryGet(out string? outputValue, "output", "o")
    ? outputValue! : Path.Combine(Directory.GetCurrentDirectory(), "output");
Directory.CreateDirectory(output);

await Console.Out.WriteLineAsync("Writing string table...");
File.WriteAllLines(Path.Combine(output, $"string-table-{version}.txt"), files);

await Console.Out.WriteLineAsync("Writing hash table...");
using FileStream fs = File.Create(Path.Combine(output, $"hash-table-{version}.json"));
JsonSerializer.Serialize(fs, hashes);

if (isFloatingConsole) {
#if RELEASE
    Console.WriteLine("""

        Press enter to exit. . .
        """);
    Console.ReadLine();
#endif
}

void FilterUntrackedFiles(List<string> src, out Dictionary<uint, string> hashes)
{
    Span<byte> data = zs.TryDecompress(msPath.RstbPath);
    Rstb rstb = new(data);

    hashes = new();

    for (int i = 0; i < src.Count; i++) {
        string name = src[i];
        if (rstb.CollisionTable.ContainsKey(name)) {
            continue;
        }

        uint hash = Crc32.Compute(name);
        if (rstb.HashTable.ContainsKey(hash)) {
            if (!hashes.TryAdd(hash, name)) {
                Console.WriteLine($"{name} >> {hashes[hash]}");
            }

            continue;
        }

        src.RemoveAt(i);
        i--;
    }
}