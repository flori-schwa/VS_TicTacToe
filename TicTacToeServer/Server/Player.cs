using TicTacToe.Core.Game;
using TicTacToe.Core.Packet.Clientbound;

namespace TicTacToe.Server {
    public class Player {
        private static int _idTicker = 0;

        private readonly ClientConnection _connection;
        private readonly string _name;
        private readonly int _id;
        private readonly PlayerType _playerType;
        private readonly Game _game;

        public Player(ClientConnection connection, string name, Game game) {
            _connection = connection;
            _name = name;
            _id = _idTicker++;
            _game = game;
            _playerType = _game.GetNextPlayerType();
        }

        public string Name => _name;

        public PlayerType PlayerType => _playerType;

        public ClientConnection Connection => _connection;
    }
}