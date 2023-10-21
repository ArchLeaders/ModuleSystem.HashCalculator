using System.Runtime.CompilerServices;
using System.Text;

namespace ModuleSystem.HashCalculator.Parsers;

public readonly ref struct RstbCollisionTable
{
    private readonly ReadOnlySpan<byte> _data;
    private readonly int _collisionStringMaxLength;
    private readonly Encoding _encoding = Encoding.UTF8;

    public RstbCollisionTable(ReadOnlySpan<byte> data, int collisionStringMaxLength)
    {
        _data = data;
        _collisionStringMaxLength = collisionStringMaxLength + 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(ReadOnlySpan<char> key)
    {
        Span<byte> binaryKey = stackalloc byte[key.Length];
        _encoding.GetBytes(key, binaryKey);

        if (binaryKey.Length > _collisionStringMaxLength - 4) {
            return false;
        }

        for (int i = 0; i < _data.Length / _collisionStringMaxLength; i++) {
            int offset = i * _collisionStringMaxLength;
            ReadOnlySpan<byte> block = _data[offset..(offset + binaryKey.Length)];

            if (block.SequenceEqual(binaryKey)) {
                return true;
            }
        }

        return false;
    }
}
