using System;

namespace TicTacToe.Core.Packet {
    public class Packet : Attribute {
        private readonly Direction _direction;
        private readonly Protocol _protocol;

        public Packet(Direction direction, Protocol protocol) {
            _direction = direction;
            _protocol = protocol;
        }

        public Direction Direction => _direction;
                
        public Protocol Protocol => _protocol;
    }
}