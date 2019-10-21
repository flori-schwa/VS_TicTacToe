using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe.Core.Packet {
    public static class PacketRegistry {
        private static readonly IDictionary<Direction, IDictionary<Type, int>> TypeToId =
            new ReadOnlyDictionary<Direction, IDictionary<Type, int>>(new Dictionary<Direction, IDictionary<Type, int>> {
                    {
                        Direction.Clientbound, new Dictionary<Type, int> {
                            {typeof(PacketS2CJoinGame), 0x00},
                            {typeof(PacketS2CAssignTurn), 0x01},
                            {typeof(PacketS2CGameOver), 0x02},
                            {typeof(PacketS2CBoardUpdate), 0x03}
                        }
                    },

                    {
                        Direction.Serverbound, new Dictionary<Type, int> {
                            {typeof(PacketC2SHello), 0x00},
                            {typeof(PacketC2SDoTurn), 0x01}
                        }
                    }
                }
            );
        private static readonly IDictionary<Direction, IDictionary<int, Type>> IdToType = 
            new ReadOnlyDictionary<Direction, IDictionary<int, Type>>( new Dictionary<Direction, IDictionary<int, Type>> {
                {
                    Direction.Clientbound, Reverse(TypeToId[Direction.Clientbound])
                },
                {
                    Direction.Serverbound, Reverse(TypeToId[Direction.Serverbound])
                }
            });

        public static int GetId(IPacket packet) => TypeToId[packet.GetDirection()][packet.GetType()];

        public static IPacket FromId(int id, Direction direction) {
            return (IPacket) IdToType[direction][id].GetConstructor(new Type[0])
                ?.Invoke(new object[0]);
        }

        private static IDictionary<TB, TA> Reverse<TA, TB>(IDictionary<TA, TB> dict) {
            return dict.ToDictionary(pair => pair.Value, pair => pair.Key);
        }
    }
}