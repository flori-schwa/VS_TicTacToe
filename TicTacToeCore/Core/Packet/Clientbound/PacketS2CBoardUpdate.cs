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

            Board.Read(reader);
        }

        public override int Write(PacketDataWriter writer) {
            return Board.Write(writer);
        }
    }
}