namespace TicTacToe.Core.Packet.Serverbound {
    [Packet(Direction.Serverbound)]
    public class PacketC2SDoTurn : IPacket {
        private int _field;

        public PacketC2SDoTurn(int field) {
            _field = field;
        }

        public PacketC2SDoTurn() {
                        
        }

        public int Field {
            get => _field;
            set => _field = value;
        }

        public void Read(PacketDataReader reader) => _field = reader.ReadInt();

        public int Write(PacketDataWriter writer) => writer.Write(_field);
    }
}