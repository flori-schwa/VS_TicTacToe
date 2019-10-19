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
        private Player _player;

        public ClientConnection(Socket socket, Game game) {
            _socket = socket;
            _game = game;
        }

        public void SendPacket(IPacket packet) {
            new Thread(() => _socket.SendPacket(packet)).Start();
        }

        private IPacket ReceivePacket() => _socket.ReceivePacket(Direction.Serverbound);

        internal void HandleClient() {
            while (_socket.Connected) {
                IPacket packet = ReceivePacket();
                    
                new Thread(() => HandlePacket(packet)).Start();
            }
        }

        private void HandlePacket(IPacket packet) {
            switch (packet) {
                case PacketC2SHello helloPacket: {
                    HandleHelloPacket(helloPacket);
                    break;
                }

                case PacketC2SDoTurn doTurnPacket: {
                    HandleDoTurnPacket(doTurnPacket);
                    break;
                }
            }
        }

        private void HandleHelloPacket(PacketC2SHello helloPacket) {
            _player = _game.RegisterPlayer(this, helloPacket.PlayerName);
        }

        private void HandleDoTurnPacket(PacketC2SDoTurn doTurnPacket) {
            _game.DoTurn(_player, doTurnPacket);
        }
    }
}