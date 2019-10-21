using System.IO;
using System.Text;

namespace TicTacToe.Core.Packet {
    public class PacketDataReader {
        private BinaryReader _reader;

        public PacketDataReader(BinaryReader reader) {
            _reader = reader;
        }

        public BinaryReader Reader => _reader;

        public bool ReadBool() => _reader.ReadBoolean();
        public byte ReadByte() => _reader.ReadByte();

        public short ReadShort() => _reader.ReadInt16();

        public int ReadInt() => _reader.ReadInt32();

        public long ReadLong() => _reader.ReadInt64();

        public string ReadString() => Encoding.UTF8.GetString(ReadByteArray(ReadInt()));

        public byte[] ReadByteArray(int count) => _reader.ReadBytes(count);

        public T Read<T>() where T : BasePacket, new() {
            T t = new T();

            t.Read(this);

            return t;
        }
    }
}