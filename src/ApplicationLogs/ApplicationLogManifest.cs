using Neo.IO;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.VM.Types;
using Neo.VM;

namespace Neo.Plugins
{
    public class ApplicationLogManifest : ISerializable
    {
        #region Manifest

        public VMState VmState { get; set; } = VMState.NONE;
        public string Exception { get; set; } = string.Empty;
        public long GasConsumed { get; set; } = 0L;
        public StackItem[] Stack { get; set; } = System.Array.Empty<StackItem>();

        #endregion

        #region Static Methods

        public static ApplicationLogManifest Create(Blockchain.ApplicationExecuted appExec) =>
            new()
            {
                VmState = appExec.VMState,
                Exception = appExec.Exception?.Message,
                GasConsumed = appExec.GasConsumed,
                Stack = appExec.Stack,
            };

        #endregion

        #region ISerializable

        public int Size =>
            sizeof(byte) +                              // VmState
            Exception.GetVarSize() +
            sizeof(long) +                              // GasConsumed
            sizeof(ushort) +                            // Length Stack Array
            sizeof(int) * Stack.Length +                // Length of each StackItem Byte Array
            CalculateStackSize();

        public void Deserialize(ref MemoryReader reader)
        {
            VmState = (VMState)reader.ReadByte();
            Exception = reader.ReadVarString();
            GasConsumed = reader.ReadInt64();

            ushort arrayLen2 = reader.ReadUInt16();
            Stack = new StackItem[arrayLen2];
            for (int i = 0; i < Stack.Length; i++)
            {
                int dataSize = reader.ReadInt32();
                Stack[i] = BinarySerializer.Deserialize(reader.ReadMemory(dataSize), ExecutionEngineLimits.Default with { MaxItemSize = 1024 * 1024 });
            }

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)VmState);
            writer.WriteVarString(Exception ?? string.Empty);
            writer.Write(GasConsumed);

            writer.Write(checked((ushort)Stack.Length));
            for (int i = 0; i < Stack.Length; i++)
            {
                var data = Stack[i] is InteropInterface ?
                    BinarySerializer.Serialize(StackItem.Null, 1024 * 1024) :
                    BinarySerializer.Serialize(Stack[i], 1024 * 1024);
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        private int CalculateStackSize()
        {
            int size = 0;
            foreach (StackItem item in Stack)
                size += item is InteropInterface ?
                    BinarySerializer.Serialize(StackItem.Null, 1024 * 1024).Length :
                    BinarySerializer.Serialize(item, 1024 * 1024).Length;
            return size;
        }

        #endregion
    }
}