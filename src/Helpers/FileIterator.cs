using SarcLibrary;
using System.Collections.Concurrent;

namespace Totk.HashCalculator.Helpers;

public class FileIterator
{
    private readonly ConcurrentBag<string> _master = new();
    private readonly TotkZstd _zs;
    private int _count = 0;

    public FileIterator(TotkZstd zs)
    {
        _zs = zs;
    }

    public static async Task<List<string>> Collect(string path, TotkZstd zs)
    {
        FileIterator iterator = new(zs);
        await iterator.Iterate(path);
        return iterator._master.Order().Distinct().ToList();
    }

    public async Task Iterate(string path)
    {
        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)) {
            string fileName = Path.GetRelativePath(path, file).Canonical();

            if (fileName.EndsWith(".bwav") || fileName.EndsWith(".hght")) {
                continue;
            }

            if (Path.GetExtension(fileName) is ".bfarc" or ".blarc" or ".genvb" or ".pack" or ".sarc" or ".ta.zs") {
                SarcFile sarc = SarcFile.FromBinary(_zs.Decompress(file).ToArray());
                foreach ((var nestedFileName, _) in sarc) {
                    _master.Add(nestedFileName.Canonical());
                    await Console.Out.WriteAsync($"\rLocated {++_count}");
                }
            }

            _master.Add(fileName);
            await Console.Out.WriteAsync($"\rLocated {++_count}");
        }
    }
}
