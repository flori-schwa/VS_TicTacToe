using System.Net.Sockets;
using System.Threading;
using TicTacToe.Core;
using TicTacToe.Core.Packet;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe.Server {
    public class ClientConnection {
        private readonly Socket _socket;
        private readonly Game _game;
        private Protocol _currentProtocol = Protocol.Handshake;
        private Player _player;

        public ClientConnection(Socket socket, Game game) {
            _socket = socket;
            _game = game;
        }

        public Protocol CurrentProtocol => _currentProtocol;

        public void SendPacket(IPacket packet) {
            new Thread(() => _socket.SendPacket(packet)).Start();
        }

        private IPacket ReceivePacket() => _socket.ReceivePacket(Direction.Serverbound);

        internal void Start() {
            while (_socket.Connected) {
                IPacket packet = ReceivePacket();
                    
                new Thread(() => HandlePacket(packet)).Start();
            }
        }

        internal void HandlePacket(IPacket packet) {
            switch (_currentProtocol) {
                case Protocol.Handshake:
                    HandleHandshakePacket(packet);
                    break;
                case Protocol.Play:
                    HandlePlayPacket(packet);
                    break;
            }
        }

        internal void HandleHandshakePacket(IPacket packet) {

            if (packet is PacketC2SHello hello) {
                // TODO Register player to game
                _player = new Player(this, hello.PlayerName, _game);
                SendPacket(new PacketS2CJoinGame(_player.PlayerType));
                    
                    
                return;
            }
                
        }

        internal void HandlePlayPacket(IPacket packet) {
                
        }
    }
}