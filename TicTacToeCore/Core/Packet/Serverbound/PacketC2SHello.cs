using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet.Serverbound {
    public class PacketC2SHello : BasePacket {
        public PacketC2SHello() { }

        public PacketC2SHello(string playerName) {
            PlayerName = playerName;
        }

        public string PlayerName { get; set; }

        public override int PacketId => 0x00;

        public override Direction Direction => Direction.Serverbound;

        public override void Read(PacketDataReader reader) {
            PlayerName = reader.ReadString();
        }

        public override int Write(PacketDataWriter writer) {
            return writer.Write(PlayerName);
        }
    }
}