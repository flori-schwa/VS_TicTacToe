using TicTacToe.Core.Game;

namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CBoardUpdate : BasePacket {

        private Board _board;

        public PacketS2CBoardUpdate(Board board) {
            _board = board;
        }

        public PacketS2CBoardUpdate() {
            
        }

        public Board Board {
            get => _board;
            set => _board = value;
        }

        public override int PacketId => 0x02;
        
        public override Direction Direction => Direction.Clientbound;

        public override void Read(PacketDataReader reader) {
            _board = new Board();

            for (int i = 0; i < 9; i++) {
                _board[i] = (PlayerType) reader.ReadByte();
            }
        }

        public override int Write(PacketDataWriter writer) {
            int writtenBytes = 0;

            for (int i = 0; i < 9; i++) {
                writtenBytes += writer.Write((byte) _board[i]);
            }

            return writtenBytes;
        }
    }
}