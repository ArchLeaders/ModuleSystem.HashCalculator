using SarcLibrary;
using ZstdSharp;

namespace Totk.HashCalculator.Helpers;

public class TotkZstd
{
    private readonly Decompressor _defaultDecompressor = new();
    private readonly Decompressor _commonDecompressor = new();
    private readonly Decompressor _mapDecompressor = new();
    private readonly Decompressor _packDecompressor = new();

    public TotkZstd(string zsDicFile)
    {
        Span<byte> data = _defaultDecompressor.Unwrap(File.ReadAllBytes(zsDicFile));
        SarcFile sarc = SarcFile.FromBinary(data.ToArray());

        _commonDecompressor.LoadDictionary(sarc["zs.zsdic"]);
        _mapDecompressor.LoadDictionary(sarc["bcett.byml.zsdic"]);
        _packDecompressor.LoadDictionary(sarc["pack.zsdic"]);
    }

    public Span<byte> Decompress(string file)
    {
        Span<byte> src = File.ReadAllBytes(file);

        if (!file.EndsWith(".zs")) {
            return src;
        }

        try {
            return
                file.EndsWith(".bcett.byml.zs") ? _mapDecompressor.Unwrap(src) :
                file.EndsWith(".pack.zs") ? _packDecompressor.Unwrap(src) :
                file.EndsWith(".rsizetable.zs") ? _defaultDecompressor.Unwrap(src) :
                _commonDecompressor.Unwrap(src);
        }
        catch (Exception ex) {
            throw new Exception($"Could not decompress '{file}'", ex);
        }
    }
}
