using TicTacToe.Core.Game;

namespace TicTacToe.Core.Packet.Clientbound {
    [Packet(Direction.Clientbound, Protocol.Handshake)]
    public class PacketS2CJoinGame : IPacket {
        private PlayerType _type;

        public PacketS2CJoinGame() { }

        public PacketS2CJoinGame(PlayerType type) {
            _type = type;
        }

        public PlayerType Type {
            get => _type;
            set => _type = value;
        }

        public void Read(PacketDataReader reader) => _type = (PlayerType) reader.ReadByte();

        public int Write(PacketDataWriter writer) => writer.Write((byte) _type);
    }
}