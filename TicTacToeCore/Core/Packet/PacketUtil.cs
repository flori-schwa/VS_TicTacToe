using System;

namespace TicTacToe.Core.Packet {
    public static class PacketUtil {
        public static Direction GetDirection(this IPacket packet) {
            Attribute[] attributes = Attribute.GetCustomAttributes(packet.GetType());

            foreach (Attribute attribute in attributes) {
                if (attribute is Packet packetInfo) {
                    return packetInfo.Direction;
                }
            }

            throw new Exception($"{packet.GetType().FullName} does not have a {typeof(Packet).FullName} attribute!");
        }
    }
}