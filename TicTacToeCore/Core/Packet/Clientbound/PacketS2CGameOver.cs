namespace TicTacToe.Core.Packet.Clientbound {
    public class PacketS2CGameOver : BasePacket {
        private bool _wonGame;

        public PacketS2CGameOver(bool wonGame) {
            _wonGame = wonGame;
        }

        public PacketS2CGameOver() { }

        public bool WonGame {
            get => _wonGame;
            set => _wonGame = value;
        }

        public override int PacketId => 0x02;
        
        public override Direction Direction => Direction.Clientbound;
        
        public override void Read(PacketDataReader reader) => _wonGame = reader.ReadBool();

        public override int Write(PacketDataWriter writer) => writer.Write(_wonGame);
    }
}