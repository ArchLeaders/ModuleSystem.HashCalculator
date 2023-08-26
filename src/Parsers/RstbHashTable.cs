using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Totk.HashCalculator.Parsers;

public readonly ref struct RstbHashTable
{
    private readonly ReadOnlySpan<byte> _data;

    public RstbHashTable(ReadOnlySpan<byte> data)
    {
        _data = data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(uint key)
    {
        int min = 0;
        int max = (_data.Length / 8) - 1;

        while (min <= max) {
            int mid = (min + max) / 2;
            int index = mid * 8;
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(_data[index..(index + 4)]);
            if (value == key) {
                return true;
            }
            else if (key < value) {
                max = mid - 1;
            }
            else {
                min = mid + 1;
            }
        }

        return false;
    }
}
