namespace TicTacToe.Core.Packet.Serverbound {
    public class PacketC2SHello : BasePacket {
        private string _playerName;

        public string PlayerName {
            get => _playerName;
            set => _playerName = value;
        }

        public PacketC2SHello() { }

        public PacketC2SHello(string playerName) {
            _playerName = playerName;
        }

        public override int PacketId => 0x00;
        
        public override Direction Direction => Direction.Serverbound;
        
        public override void Read(PacketDataReader reader) => _playerName = reader.ReadString();

        public override int Write(PacketDataWriter writer) => writer.Write(_playerName);
    }
}