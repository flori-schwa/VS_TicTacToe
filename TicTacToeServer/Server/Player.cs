using TicTacToe.Core.Game;

namespace TicTacToe.Server {
    public class Player {
        private static int _idTicker;

        private readonly Game _game;
        private readonly int _id;

        public Player(ClientConnection connection, string name, Game game) {
            Connection = connection;
            Name = name;
            _id = _idTicker++;
            _game = game;
            PlayerType = _game.GetNextPlayerType();
        }

        public string Name { get; }

        public PlayerType PlayerType { get; }

        public ClientConnection Connection { get; }
    }
}