namespace TicTacToe.Core.Packet {
    public interface IPacket {
        void Read(PacketDataReader reader);

        int Write(PacketDataWriter writer);
    }
}