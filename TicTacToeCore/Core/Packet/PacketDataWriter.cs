using System.IO;
using System.Text;

namespace TicTacToe.Core.Packet {
    public class PacketDataWriter {
        private BinaryWriter _writer;

        public PacketDataWriter(BinaryWriter writer) {
            _writer = writer;
        }

        public int Write(byte b) {
            _writer.Write(b);
                    
            return sizeof(byte);
        }

        public int Write(short s) {
            _writer.Write(s);
                    
            return sizeof(short);
        }

        public int Write(int i) {
            _writer.Write(i);

            return sizeof(int);
        }

        public int Write(long l) {
            _writer.Write(l);

            return sizeof(long);
        }

        public int Write(byte[] bytes) {
            _writer.Write(bytes);

            return bytes.Length;
        }

        public int Write(string s) => Write(Encoding.UTF8.GetBytes(s));

        public int Write<T>(T t) where T : IPacket => t.Write(this);

        public void Flush() => _writer.Flush();
    }
}