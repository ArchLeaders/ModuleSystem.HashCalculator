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
        await iterator.IterateParallel(path);
        return iterator._master.Order().Distinct().ToList();
    }

    public async Task IterateParallel(string path)
    {
        IEnumerable<string> folders = Directory.EnumerateDirectories(path);
        await Parallel.ForEachAsync(folders, async (folder, cancellationToken) =>
        {
            await Iterate(folder, path);
        });
    }

    public async Task Iterate(string path, string root)
    {
        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {
            string fileName = Path.GetRelativePath(root, file).Canonical();

            if (Path.GetExtension(fileName) is ".bfarc" or ".blarc" or ".genvb" or ".pack" or ".sarc" or ".ta")
            {
                SarcFile sarc = SarcFile.FromBinary(_zs.Decompress(file).ToArray());
                foreach ((var nestedFileName, _) in sarc)
                {
                    _master.Add(nestedFileName.Canonical());
                    await Console.Out.WriteAsync($"\rLocated {++_count}");
                }
            }

            _master.Add(fileName);
            await Console.Out.WriteAsync($"\rLocated {++_count}");
        }
    }
}
