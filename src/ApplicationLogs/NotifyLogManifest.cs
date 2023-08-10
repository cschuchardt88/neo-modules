using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Plugins
{
    public class NotifyLogManifest : ISerializable
    {
        #region Manifest

        public UInt256 BlockHash { get; set; } = new();
        public UInt256 TransactionHash { get; set; } = new();
        public UInt160 ScriptHash { get; set; } = new();
        public string EventName { get; set; } = string.Empty;
        public StackItem[] State { get; set; } = System.Array.Empty<StackItem>();

        #endregion

        #region Static Methods

        public static NotifyLogManifest Create(NotifyEventArgs notifyArgs, UInt256 blockHash) =>
            new()
            {
                BlockHash = blockHash,
                TransactionHash = ((Transaction)notifyArgs.ScriptContainer).Hash,
                ScriptHash = notifyArgs.ScriptHash,
                EventName = notifyArgs.EventName,
                State = notifyArgs.State.ToArray(),
            };

        public static NotifyLogManifest Create(NotifyEventArgs notifyArgs, UInt256 blockHash, UInt256 txHash) =>
            new()
            {
                BlockHash = blockHash,
                TransactionHash = txHash,
                ScriptHash = notifyArgs.ScriptHash,
                EventName = notifyArgs.EventName,
                State = notifyArgs.State.ToArray(),
            };

        #endregion

        #region ISerializable

        public int Size =>
            BlockHash.Size +
            TransactionHash.Size +
            ScriptHash.Size +
            EventName.GetVarSize() +
            sizeof(ushort) +                // Length of StackItems Array
            sizeof(int) * State.Length +   // Length of each StackItem Byte Array
            CalculateStateSize();

        public void Deserialize(ref MemoryReader reader)
        {
            BlockHash.Deserialize(ref reader);
            TransactionHash.Deserialize(ref reader);
            ScriptHash.Deserialize(ref reader);
            EventName = reader.ReadVarString();

            ushort arraylen = reader.ReadUInt16();
            State = new StackItem[arraylen];
            for (int i = 0; i < State.Length; i++)
            {
                int dataSize = reader.ReadInt32();
                State[i] = BinarySerializer.Deserialize(reader.ReadMemory(dataSize), ExecutionEngineLimits.Default with { MaxItemSize = uint.MaxValue / 2 });
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(BlockHash);
            writer.Write(TransactionHash);
            writer.Write(ScriptHash);
            writer.WriteVarString(EventName);
            writer.Write(checked((ushort)State.Length));
            for (int i = 0; i < State.Length; i++)
            {
                byte[] data = System.Array.Empty<byte>();
                try
                {
                    data = BinarySerializer.Serialize(State[i], uint.MaxValue / 2);
                }
                catch
                {
                    data = BinarySerializer.Serialize(StackItem.Null, uint.MaxValue / 2);
                }
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        private int CalculateStateSize()
        {
            int size = 0;
            foreach (StackItem item in State)
            {
                try
                {
                    size += BinarySerializer.Serialize(item, uint.MaxValue / 2).Length;
                }
                catch
                {
                    size += BinarySerializer.Serialize(StackItem.Null, uint.MaxValue / 2).Length;
                }
            }
            return size;
        }

        #endregion
    }
}
