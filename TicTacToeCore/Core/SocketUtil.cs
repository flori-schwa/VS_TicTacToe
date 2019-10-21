using System;
using System.IO;
using System.Net.Sockets;
using TicTacToe.Core.Packet;

namespace TicTacToe.Core {
    public static class SocketUtil {
        public static void SendPacket(this Socket socket, IPacket packet) {
            MemoryStream packetDataStream = new MemoryStream();
            PacketDataWriter packetDataWriter = new PacketDataWriter(new BinaryWriter(packetDataStream));

            int packetDataLength = packetDataWriter.Write(packet);
            int totalPacketLength = 2 * sizeof(int) + packetDataLength; // Packet id + Packet data length + Packet data

            packetDataWriter.Flush();
            byte[] packetData = packetDataStream.GetBuffer();
            packetDataStream.Close();

            byte[] fullPacketBuffer = new byte[totalPacketLength];
            MemoryStream fullPacketData = new MemoryStream(fullPacketBuffer);
            BinaryWriter packetWriter = new BinaryWriter(fullPacketData);

            packetWriter.Write(PacketRegistry.GetId(packet));
            packetWriter.Write(packetDataLength);
            packetWriter.Write(packetData, 0, packetDataLength);

            socket.Send(fullPacketBuffer);
        }

        public static IPacket ReceivePacket(this Socket socket, Direction direction) {
            Span<byte> intBuffer = stackalloc byte[sizeof(int)];
            socket.Receive(intBuffer);
            int packetId = BitConverter.ToInt32(intBuffer);

            IPacket packet = PacketRegistry.FromId(packetId, direction);

            socket.Receive(intBuffer);
            int dataLength = BitConverter.ToInt32(intBuffer);

            byte[] packetDataBuffer = new byte[dataLength];
            socket.Receive(packetDataBuffer);

            MemoryStream memoryStream = new MemoryStream(packetDataBuffer, false);
            packet.Read(new PacketDataReader(new BinaryReader(memoryStream)));

            return packet;
        }
    }
}