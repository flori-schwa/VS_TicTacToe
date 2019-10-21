using TicTacToe.Core.Game;

namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CJoinGame : BasePacket {
        private PlayerType _type;

        public PacketS2CJoinGame() { }

        public PacketS2CJoinGame(PlayerType type) {
            _type = type;
        }

        public PlayerType Type {
            get => _type;
            set => _type = value;
        }

        public override int PacketId => 0x00;
        
        public override Direction Direction => Direction.Clientbound;
        
        public override void Read(PacketDataReader reader) => _type = (PlayerType) reader.ReadByte();

        public override int Write(PacketDataWriter writer) => writer.Write((byte) _type);
    }
}