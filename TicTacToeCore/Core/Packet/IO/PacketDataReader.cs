using System.IO;
using System.Text;

namespace TicTacToe.Core.Packet.IO {
    public class PacketDataReader {
        public PacketDataReader(BinaryReader reader) {
            Reader = reader;
        }

        public BinaryReader Reader { get; }

        public bool ReadBool() {
            return Reader.ReadBoolean();
        }

        public byte ReadByte() {
            return Reader.ReadByte();
        }

        public short ReadShort() {
            return Reader.ReadInt16();
        }

        public int ReadInt() {
            return Reader.ReadInt32();
        }

        public long ReadLong() {
            return Reader.ReadInt64();
        }

        public string ReadString() {
            return Encoding.UTF8.GetString(ReadByteArray(ReadInt()));
        }

        public byte[] ReadByteArray(int count) {
            return Reader.ReadBytes(count);
        }

        public T Read<T>() where T : IProtocolReadWrite, new() {
            T t = new T();

            t.Read(this);

            return t;
        }
    }
}