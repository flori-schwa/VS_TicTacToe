using System;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Core;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe.Client {
    public class TicTacToeClient {
        public static void Main(string[] args) {
            new TicTacToeClient();
        }

        private readonly Socket _clientSocket;

        private TicTacToeClient() {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 1337);

            _clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.Connect(ipEndPoint);


            Console.WriteLine("Enter your Username: ");
            string username = Console.ReadLine();

            // Initialize with Hello Packet
            _clientSocket.SendPacket(new PacketC2SHello(username));
        }
    }
}