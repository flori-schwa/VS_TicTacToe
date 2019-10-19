namespace TicTacToe.Core.Packet.Clientbound {
    [Packet(Direction.Clientbound)]
    public class PacketS2CGameOver : IPacket {
        private bool _wonGame;

        public PacketS2CGameOver(bool wonGame) {
            _wonGame = wonGame;
        }

        public PacketS2CGameOver() { }

        public bool WonGame {
            get => _wonGame;
            set => _wonGame = value;
        }

        public void Read(PacketDataReader reader) => _wonGame = reader.ReadBool();

        public int Write(PacketDataWriter writer) => writer.Write(_wonGame);
    }
}