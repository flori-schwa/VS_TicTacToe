namespace TicTacToe.Core.Packet.Serverbound {
    [Packet(Direction.Serverbound)]
    public class PacketC2SHello : IPacket {
        private string _playerName;

        public string PlayerName {
            get => _playerName;
            set => _playerName = value;
        }

        public PacketC2SHello() { }

        public PacketC2SHello(string playerName) {
            _playerName = playerName;
        }

        public void Read(PacketDataReader reader) => _playerName = reader.ReadString();

        public int Write(PacketDataWriter writer) => writer.Write(_playerName);
    }
}