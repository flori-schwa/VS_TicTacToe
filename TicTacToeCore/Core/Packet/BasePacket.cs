using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet {
    public abstract class BasePacket {
        public abstract int PacketId { get; }

        public abstract Direction Direction { get; }

        public abstract void Read(PacketDataReader reader);

        public abstract int Write(PacketDataWriter writer);
    }
}