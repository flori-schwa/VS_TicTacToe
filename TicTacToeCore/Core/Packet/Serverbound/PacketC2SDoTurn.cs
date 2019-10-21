using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet.Serverbound {
    public class PacketC2SDoTurn : BasePacket {
        public PacketC2SDoTurn(int field) {
            Field = field;
        }

        public PacketC2SDoTurn() { }

        public int Field { get; set; }

        public override int PacketId => 0x01;

        public override Direction Direction => Direction.Serverbound;

        public override void Read(PacketDataReader reader) {
            Field = reader.ReadInt();
        }

        public override int Write(PacketDataWriter writer) {
            return writer.Write(Field);
        }
    }
}