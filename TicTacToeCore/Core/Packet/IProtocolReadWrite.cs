using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet {
    public interface IProtocolReadWrite {
        void Read(PacketDataReader reader);

        int Write(PacketDataWriter writer);
    }
}