using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TicTacToe.Server {
    public class TicTacToeServer {
        private readonly Game _game = new Game();

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
                }
                catch (SocketException e) {
                    if (e.SocketErrorCode == SocketError.Interrupted) continue; // Ignore

                    Console.WriteLine(e);
                    continue;
                }

                new Thread(() => {
                    if (_game.IsGameFull()) {
                        clientSocket.Close();
                        return;
                    }

                    new ClientConnection(clientSocket, _game).HandleClient();
                }).Start();
            }
        }
    }
}