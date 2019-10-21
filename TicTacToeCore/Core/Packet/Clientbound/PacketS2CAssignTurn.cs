namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CAssignTurn : BasePacket {
                    
        private int[] _availableFields;

        public PacketS2CAssignTurn(int[] availableFields) {
            _availableFields = availableFields;
        }

        public PacketS2CAssignTurn() {
                        
        }

        public int[] AvailableFields {
            get => _availableFields;
            set => _availableFields = value;
        }

        public override int PacketId => 0x01;

        public override Direction Direction => Direction.Clientbound;

        public override void Read(PacketDataReader reader) {
            _availableFields = new int[reader.ReadInt()];

            for (int i = 0; i < _availableFields.Length; i++) {
                _availableFields[i] = reader.ReadInt();
            }
        }

        public override int Write(PacketDataWriter writer) {
            int written = writer.Write(_availableFields.Length);

            foreach (int availableField in _availableFields) {
                written += writer.Write(availableField);
            }

            return written;
        }
    }
}