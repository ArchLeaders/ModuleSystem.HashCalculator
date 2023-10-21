using SarcLibrary;
using ZstdSharp;

namespace ModuleSystem.HashCalculator.Helpers;

public class ModuleSystemZstd
{
    private readonly Decompressor _defaultDecompressor = new();
    private readonly Dictionary<string, Decompressor> _decompressors = new();

    public ModuleSystemZstd(string zsDicFile)
    {
        if (File.Exists(zsDicFile)) {
            Span<byte> data = _defaultDecompressor.Unwrap(File.ReadAllBytes(zsDicFile));
            SarcFile sarc = SarcFile.FromBinary(data.ToArray());

            foreach ((var file, var fileData) in sarc) {
                Decompressor decompressor = new();
                decompressor.LoadDictionary(fileData);
                _decompressors[file[..file.LastIndexOf('.')]] = decompressor;
            }
        }
    }

    public Span<byte> TryDecompress(string file)
    {
        Span<byte> src = File.ReadAllBytes(file);

        if (!file.EndsWith(".zs")) {
            return src;
        }

        try {
            foreach ((var key, var decompressor) in _decompressors) {
                if (file.EndsWith($"{key}.zs")) {
                    return decompressor.Unwrap(src);
                }
            }

            return _defaultDecompressor.Unwrap(src);
        }
        catch (Exception ex) {
            throw new Exception($"Could not decompress '{file}'", ex);
        }
    }
}
