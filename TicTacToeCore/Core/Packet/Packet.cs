using System;

namespace TicTacToe.Core.Packet {
    public class Packet : Attribute {
        private readonly Direction _direction;

        public Packet(Direction direction) {
            _direction = direction;
        }

        public Direction Direction => _direction;
    }
}