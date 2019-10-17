using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TicTacToe.Core;
using TicTacToe.Core.Game;
using TicTacToe.Core.Packet;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.Serverbound;
using TicTacToe.Server;

namespace TicTacToe {
    public static class TicTacToeServerMain {
        public static void Main(string[] args) {
            new TicTacToeServer();
        }
    }
    
    namespace Server {
        public class TicTacToeServer {
            private Game _currentGame = new Game();
            
            internal TicTacToeServer() {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 1337);
            
                Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(10);

                bool run = true;

                Console.CancelKeyPress += (sender, args) => {
                    run = false;
                    serverSocket.Close();

                    Console.WriteLine("Server closed.");
                    Thread.Sleep(1000);
                };

                Console.WriteLine("Press CTRL-C to Stop the server");
                
                while (run) {
                    Socket clientSocket;
                    
                    try {
                        clientSocket = serverSocket.Accept();
                    } catch (SocketException e) {
                        if (e.SocketErrorCode == SocketError.Interrupted) {
                            continue; // Ignore
                        }
                        
                        Console.WriteLine(e);
                        continue;
                    }

                    new Thread(() => {
                        if (_currentGame.IsGameFull()) {
                            clientSocket.Close();
                            return;
                        }
                        
                        new ClientConnection(clientSocket, _currentGame).Start();
                    }).Start();
                }
            }
        }

        internal class Game {
            private static readonly int[][] WinChecks = {
                // Horizontals
                new[] {0, 0, 1, 0},
                new[] {0, 1, 1, 0},
                new[] {0, 2, 1, 0},
                
                // Verticals
                new[] {0, 0, 0, 1},
                new[] {1, 0, 0, 1},
                new[] {2, 0, 0, 1},
                
                
                // Diagonals
                new[] {0, 0, 1, 1},
                new[] {0, 2, 1, -1},
            };

            private class Board {
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
            }

            private readonly Board _board = new Board();
            private Player[] _players = new Player[2];
            
            private Player _currentTurn;

            public void AssignTurn(Player player) {
                _currentTurn = player;
                List<int> availableFields = new List<int>();

                for (int i = 0; i < _board.Length; i++) {
                    if (_board[i] == PlayerType.None) {
                        availableFields.Add(i);
                    }
                }
                
                player.Connection.SendPacket(new PacketS2CAssignTurn(availableFields.ToArray()));
            }

            public bool IsGameFull() => _players[0] != null && _players[1] != null;

            public PlayerType GetNextPlayerType() {
                if (!IsPlayerTypeTaken(PlayerType.X)) {
                    return PlayerType.X;
                }

                if (!IsPlayerTypeTaken(PlayerType.O)) {
                    return PlayerType.O;
                }

                throw new Exception($"Game is full!");
            }

            private bool IsPlayerTypeTaken(PlayerType type) {
                return _players[0]?.PlayerType == type || _players[1]?.PlayerType == type;
            }

            public void DoTurn(Player player, PacketC2SDoTurn doTurnPacket) {
                if (_currentTurn != player) {
                    return; // Ignore
                }

                if (_board[doTurnPacket.Field] != PlayerType.None) {
                    return; // TODO Handle Illegal Move
                }

                _board[doTurnPacket.Field] = _currentTurn.PlayerType;

                if (CheckWinner(player)) {
                    Win(player);
                }
                else {
                    AssignTurn(GetPlayerByType(GetReverseType(player.PlayerType)));
                }
            }

            private PlayerType GetReverseType(PlayerType playerType) {
                switch (playerType) {
                    case PlayerType.X:
                        return PlayerType.O;
                    case PlayerType.O:
                        return PlayerType.X;
                    default:
                        throw new ArgumentException();
                }
            }

            private Player GetPlayerByType(PlayerType type) {
                return _players[0].PlayerType == type ? _players[0] : _players[1];
            }

            private bool CheckWinner(Player player) {
                PlayerType type = player.PlayerType;
                
                bool WonLine(int x, int y, int dx, int dy) {
                    for (int i = 0; i < 3; i++) {
                        if (_board[x, y] != type) {
                            return false;
                        }

                        x += dx;
                        y += dy;
                    }

                    return true;
                }

                foreach (int[] winCheck in WinChecks) {
                    if (WonLine(winCheck[0], winCheck[1], winCheck[2], winCheck[3])) {
                        return true;
                    }
                }

                return false;
            }

            private void Win(Player player) {
                
            }



        }

        internal class ClientConnection {
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

        internal class Player {
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
}