using TicTacToe.Core.Packet;
using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Game {
    public class Board : IProtocolReadWrite {
        private readonly PlayerType[] _board = {
            PlayerType.None,
            PlayerType.None,
            PlayerType.None,

            PlayerType.None,
            PlayerType.None,
            PlayerType.None,

            PlayerType.None,
            PlayerType.None,
            PlayerType.None
        };

        public PlayerType this[int row, int column] {
            get => _board[row * 3 + column];
            set => _board[row * 3 + column] = value;
        }

        public PlayerType this[int index] {
            get => _board[index];
            set => _board[index] = value;
        }

        public int Length => _board.Length;
        
        public void Read(PacketDataReader reader) {
            for (int i = 0; i < Length; i++) {
                _board[i] = (PlayerType) reader.ReadByte();
            }
        }

        public int Write(PacketDataWriter writer) {
            for (int i = 0; i < Length; i++) {
                writer.Write((byte) _board[i]);
            }

            return Length;
        }
    }
}