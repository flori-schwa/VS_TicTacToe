using TicTacToe.Core.Game;
using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CBoardUpdate : BasePacket {
        public PacketS2CBoardUpdate(Board board) {
            Board = board;
        }

        public PacketS2CBoardUpdate() { }

        public Board Board { get; set; }

        public override int PacketId => 0x02;

        public override Direction Direction => Direction.Clientbound;

        public override void Read(PacketDataReader reader) {
            Board = new Board();

            for (int i = 0; i < 9; i++) Board[i] = (PlayerType) reader.ReadByte();
        }

        public override int Write(PacketDataWriter writer) {
            int writtenBytes = 0;

            for (int i = 0; i < 9; i++) writtenBytes += writer.Write((byte) Board[i]);

            return writtenBytes;
        }
    }
}