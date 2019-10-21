using TicTacToe.Core.Packet.IO;

namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CAssignTurn : BasePacket {
        public PacketS2CAssignTurn(int[] availableFields) {
            AvailableFields = availableFields;
        }

        public PacketS2CAssignTurn() { }

        public int[] AvailableFields { get; set; }

        public override int PacketId => 0x01;

        public override Direction Direction => Direction.Clientbound;

        public override void Read(PacketDataReader reader) {
            AvailableFields = new int[reader.ReadInt()];

            for (int i = 0; i < AvailableFields.Length; i++) AvailableFields[i] = reader.ReadInt();
        }

        public override int Write(PacketDataWriter writer) {
            int written = writer.Write(AvailableFields.Length);

            foreach (int availableField in AvailableFields) written += writer.Write(availableField);

            return written;
        }
    }
}