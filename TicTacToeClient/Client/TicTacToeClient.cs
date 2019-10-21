using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TicTacToe.Core;
using TicTacToe.Core.Game;
using TicTacToe.Core.Packet;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.IO;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe.Client {
    public class TicTacToeClient {
        private readonly Socket _clientSocket;

        private TicTacToeClient() {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 1337);

            _clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.Connect(ipEndPoint);


            Console.WriteLine("Enter your Username: ");
            string username = Console.ReadLine();

            // Initialize with Hello Packet
            _clientSocket.SendPacket(new PacketC2SHello(username));

            new Thread(() => {
                while (true) {
                    BasePacket packet = _clientSocket.ReceivePacket(Direction.Clientbound);

                    new Thread(() => HandlePacket(packet)).Start();
                }
            }).Start();

            while (true)
                try {
                    int field = int.Parse(Console.ReadLine());

                    _clientSocket.SendPacket(new PacketC2SDoTurn(field));
                }
                catch (Exception e) { }

            // TODO Literally everything else
        }

        public static void Main(string[] args) {
            new TicTacToeClient();
        }

        private string PlayerTypeToChar(PlayerType playerType) {
            switch (playerType) {
                case PlayerType.O:
                    return "O";
                case PlayerType.X:
                    return "X";
            }

            return " ";
        }

        private void HandlePacket(BasePacket packet) {
            switch (packet) {
                case PacketS2CJoinGame joinGamePacket: {
                    Console.WriteLine($"You are playing as {joinGamePacket.Type}");
                    break;
                }

                case PacketS2CAssignTurn assignTurnPacket: {
                    Console.WriteLine(
                        $"Its your turn! Available fields: {assignTurnPacket.AvailableFields.CollectionToString()}");
                    break;
                }

                case PacketS2CGameOver gameOverPacket: {
                    if (gameOverPacket.WonGame)
                        Console.WriteLine("You won the game!");
                    else
                        Console.WriteLine("You lost the game!");

                    break;
                }

                case PacketS2CBoardUpdate boardUpdatePacket: {
                    Board board = boardUpdatePacket.Board;

                    for (int y = 0; y < 3; y++) {
                        for (int x = 0; x < 3; x++) Console.Write(PlayerTypeToChar(board[y, x]));

                        Console.WriteLine();
                    }

                    break;
                }
            }
        }
    }
}