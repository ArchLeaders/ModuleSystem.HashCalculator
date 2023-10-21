using System.Buffers.Binary;

namespace ModuleSystem.HashCalculator.Parsers;

public readonly ref struct Rstb
{
    public readonly RstbHashTable HashTable;
    public readonly RstbCollisionTable CollisionTable;

    public Rstb(ReadOnlySpan<byte> data)
    {
        int hashEntryCount = BinaryPrimitives.ReadInt32LittleEndian(data[14..18]);
        int hashTableSize = hashEntryCount * 8;

        HashTable = new(data[22..hashTableSize]);
        CollisionTable = new(data[(22 + hashTableSize)..],
            BinaryPrimitives.ReadInt32LittleEndian(data[10..14]));
    }
}
