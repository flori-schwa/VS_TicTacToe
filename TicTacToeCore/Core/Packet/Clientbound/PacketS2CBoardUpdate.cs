using TicTacToe.Core.Game;

namespace TicTacToe.Core.Packet.Clientbound {
    [Packet(Direction.Clientbound)]
    public class PacketS2CBoardUpdate : IPacket {

        private PlayerType[] _board;

        public PacketS2CBoardUpdate(PlayerType[] board) {
            _board = board;
        }

        public PacketS2CBoardUpdate() {
            
        }

        public PlayerType[] Board {
            get => _board;
            set => _board = value;
        }

        public void Read(PacketDataReader reader) {
            _board = new PlayerType[9];

            for (int i = 0; i < 9; i++) {
                _board[i] = (PlayerType) reader.ReadByte();
            }
        }

        public int Write(PacketDataWriter writer) {
            int writtenBytes = 0;

            for (int i = 0; i < 9; i++) {
                writtenBytes += writer.Write((byte) _board[i]);
            }

            return writtenBytes;
        }
    }
}