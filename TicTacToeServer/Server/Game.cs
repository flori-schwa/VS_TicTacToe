using System;
using System.Collections.Generic;
using TicTacToe.Core.Game;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe.Server {
    public class Game {
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

        public void DoTurn(Player player, PacketC2SDoTurn doTurnPacket) {
            if (_currentTurn != player) {
                return; // Ignore
            }

            if (_board[doTurnPacket.Field] != PlayerType.None) {
                return; // TODO Handle Illegal Move
            }

            _board[doTurnPacket.Field] = _currentTurn.PlayerType;
            BroadcastBoard();
            
            if (CheckWinner(player)) {
                Win(player);
            }
            else {
                AssignTurn(GetPlayerByType(GetReverseType(player.PlayerType)));
            }
        }

        private void BroadcastBoard() {
            PacketS2CBoardUpdate updatePacket = new PacketS2CBoardUpdate(_board.BoardArray);
            
            _players[0].Connection.SendPacket(updatePacket);
            _players[1].Connection.SendPacket(updatePacket);
        }

        private bool IsPlayerTypeTaken(PlayerType type) {
            return _players[0]?.PlayerType == type || _players[1]?.PlayerType == type;
        }

        public Player RegisterPlayer(ClientConnection connection, string name) {
            for (int i = 0; i < 2; i++) {
                if (_players[i] != null) {
                    continue;
                }

                Player player = new Player(connection, name, this);
                player.Connection.SendPacket(new PacketS2CJoinGame(player.PlayerType));
                _players[i] = player;

                if (IsGameFull()) {
                    StartGame();
                }
                    
                return player;
            }
            
            throw new Exception("Game already full!");
        }

        private void StartGame() {
            AssignTurn(_players[0]);
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

        private Player GetOtherPlayer(Player player) {
            if (_players[0] == player) {
                return _players[1];
            }

            if (_players[1] == player) {
                return _players[0];
            }
            
            throw new ArgumentException($"Player {player.Name} is not part of this game!");
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
            player.Connection.SendPacket(new PacketS2CGameOver(true));
            
            GetOtherPlayer(player).Connection.SendPacket(new PacketS2CGameOver(false));
        }
    }
}