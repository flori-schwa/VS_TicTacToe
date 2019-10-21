using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CGameOver : BasePacket {
        public PacketS2CGameOver(bool wonGame) {
            WonGame = wonGame;
        }

        public PacketS2CGameOver() { }

        public bool WonGame { get; set; }

        public override int PacketId => 0x03;

        public override Direction Direction => Direction.Clientbound;

        public override void Read(PacketDataReader reader) {
            WonGame = reader.ReadBool();
        }

        public override int Write(PacketDataWriter writer) {
            return writer.Write(WonGame);
        }
    }
}