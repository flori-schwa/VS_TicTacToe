namespace TicTacToe.Core.Game {
    public class Board {
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

        public PlayerType[] BoardArray => _board;
    }
}