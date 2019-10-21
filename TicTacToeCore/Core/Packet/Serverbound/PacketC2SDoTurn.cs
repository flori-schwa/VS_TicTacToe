namespace TicTacToe.Core.Packet.Serverbound {
    public class PacketC2SDoTurn : BasePacket {
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

        public override int PacketId => 0x01;
        
        public override Direction Direction => Direction.Serverbound;
        
        public override void Read(PacketDataReader reader) => _field = reader.ReadInt();

        public override int Write(PacketDataWriter writer) => writer.Write(_field);
    }
}