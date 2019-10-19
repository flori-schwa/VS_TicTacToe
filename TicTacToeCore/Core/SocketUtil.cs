using System;
using System.IO;
using System.Net.Sockets;
using TicTacToe.Core.Packet;

namespace TicTacToe.Core {
    public static class SocketUtil {
            public static void SendPacket(this Socket socket, IPacket packet) {
                MemoryStream packetData = new MemoryStream();
                PacketDataWriter packetDataWriter = new PacketDataWriter(new BinaryWriter(packetData));
                
                int packetDataLength = packetDataWriter.Write(packet);
                int totalLength = 2 * sizeof(int) + packetDataLength; // Packet id + Packet data length + Packet data
                
                packetDataWriter.Flush();
                byte[] data = packetData.GetBuffer();
                packetData.Close();
                
                byte[] packetBuffer = new byte[totalLength];
                packetData = new MemoryStream(packetBuffer);
                BinaryWriter packetWriter = new BinaryWriter(packetData);
                
                packetWriter.Write(PacketRegistry.GetId(packet));
                packetWriter.Write(packetDataLength);
                packetWriter.Write(data, 0, packetDataLength);

                socket.Send(packetBuffer);
            }

            public static IPacket ReceivePacket(this Socket socket, Direction direction) {
                BinaryReader reader = socket.Reader();

                int packetId = reader.ReadInt32();
                
                IPacket packet = PacketRegistry.FromId(packetId, direction);
                
                int dataLength = reader.ReadInt32();

                byte[] dataBuffer = reader.ReadBytes(dataLength);
                
                MemoryStream memoryStream = new MemoryStream(dataBuffer, false);
                packet.Read(new PacketDataReader(new BinaryReader(memoryStream)));

                return packet;
            }

            public static BinaryReader Reader(this Socket socket, int bufferSize = 256) {
                if (bufferSize <= 0) {
                    throw new ArgumentException($"{nameof(bufferSize)} must be > 0");
                }

                MemoryStream memoryStream = new MemoryStream();

                byte[] buffer = new byte[bufferSize];
                int read;

                do {
                    read = socket.Receive(buffer);
                    memoryStream.Write(buffer, 0, read);
                } while (read > 256);

                memoryStream.Position = 0;

                return new BinaryReader(memoryStream);
            }
        }
}