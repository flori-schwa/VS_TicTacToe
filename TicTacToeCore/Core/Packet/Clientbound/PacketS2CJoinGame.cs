using TicTacToe.Core.Game;
using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CJoinGame : BasePacket {
        public PacketS2CJoinGame() { }

        public PacketS2CJoinGame(PlayerType type) {
            Type = type;
        }

        public PlayerType Type { get; set; }

        public override int PacketId => 0x00;

        public override Direction Direction => Direction.Clientbound;

        public override void Read(PacketDataReader reader) {
            Type = (PlayerType) reader.ReadByte();
        }

        public override int Write(PacketDataWriter writer) {
            return writer.Write((byte) Type);
        }
    }
}